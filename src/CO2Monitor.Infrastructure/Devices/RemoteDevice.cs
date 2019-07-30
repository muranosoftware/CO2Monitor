using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using Json = Newtonsoft.Json.Linq;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public class RemoteDevice : IRemoteDevice {
		private string _name;
		private float _pollingRate;
		private Uri _address;
		private readonly Timer _timer;

		private DeviceInfo _baseInfo;
		private DeviceInfo _info;
		private readonly ILogger _logger;
		private readonly IDeviceStateRepository _stateRepository;
		
		public RemoteDevice(ILogger<RemoteDevice> logger, IDeviceStateRepository repository) {
			_baseInfo = new DeviceInfo();
			_info = new DeviceInfo();
			_pollingRate = 60.0f;
			_timer = new Timer(MakeStateRequest, null, TimeSpan.FromSeconds(_pollingRate), TimeSpan.FromSeconds(_pollingRate));
			_logger = logger;
			_stateRepository = repository;
		}

		public int Id { get; set; }

		public string Name {
			get => _name;
			set {
				if (_name != value) {
					_name = value;
					SettingsChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
				}
			}
		}

		public float PollingRate {
			get => _pollingRate;
			set {
				if (Math.Abs(value - _pollingRate) > 1e-7) {
					_pollingRate = value;
					OnSettingsChanged("PollingRate");
					_timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_pollingRate));
				}
			}
		}

		public DeviceInfo BaseInfo {
			get => _baseInfo;
			set {
				_baseInfo = value;
				UpdateInfo();
			}
		}

		public Uri Address {
			get => _address;
			set {
				if (value != _address) {
					_address = value;
					OnSettingsChanged("Address");
					MakeStateRequest(null);
				}
			}
		}

		public DateTime? LatestSuccessfulAccess { get; set; }

		public RemoteDeviceStatus Status { get; set; } = RemoteDeviceStatus.NotAccessible;

		public List<IDeviceExtension> Extensions { get; set; } = new List<IDeviceExtension>();

		IEnumerable<IDeviceExtension> IExtendableDevice.Extensions => Extensions;
		
		public string DeviceState { get; set; }

		public string State {
			get {
				Json.JObject json = Json.JObject.Parse(DeviceState ?? "{}");
				foreach (IDeviceExtension extension in Extensions) {
					Json.JObject extJson = Json.JObject.Parse(extension.State ?? "{}");
					foreach (Json.JProperty jp in extJson.Properties()) {
						json.Add(jp.Name, jp.Value);
					}
				}
				return json.ToString();
			}
		}

		public DeviceInfo Info => _info;

		public event PropertyChangedEventHandler SettingsChanged;
		public event DeviceEventHandler EventRaised;

		public void AddExtension(IDeviceExtension extension) {
			extension.EventRaised += ExtensionEventRaised;
			extension.SettingsChanged += ExtensionSettingsChanged;
			Extensions.Add(extension);
			UpdateInfo();
			SettingsChanged?.Invoke(this, new PropertyChangedEventArgs("Extensions"));
		}

		public void Dispose() {
			_timer.Dispose();
			foreach (IDeviceExtension ext in Extensions) {
				ext.Dispose();
			}
		}

		public void UpdateInfo() {
			List<DeviceStateFieldDeclaration> fields = _baseInfo.Fields.ToList();
			List<DeviceActionDeclaration> actions = _baseInfo.Actions.ToList();
			List<DeviceEventDeclaration> events = _baseInfo.Events.ToList();

			foreach (IDeviceExtension e in Extensions) {
				fields.AddRange(e.BaseInfo.Fields);
				actions.AddRange(e.BaseInfo.Actions);
				events.AddRange(e.BaseInfo.Events);
			}

			_info = new DeviceInfo(fields, actions, events);
		}

		public async Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Variant value) {
			foreach (IDeviceExtension e in Extensions) {
				if (e.BaseInfo.Actions.Contains(deviceActionDeclaration)) {
					await e.ExecuteAction(deviceActionDeclaration, value);
					return;
				}
			}

			await ExecuteActionRemote(deviceActionDeclaration, value);
		}

		private async Task ExecuteActionRemote(DeviceActionDeclaration deviceActionDeclaration, Variant value) {
			var url = new Uri(Address + deviceActionDeclaration.Path);

			if (value != null && value.Declaration.Type != VariantType.Void) {
				url = new Uri(url + "/" + value);
			}

			using (var client = new HttpClient()) {
				client.Timeout = TimeSpan.FromSeconds(30);
			
				try {
					HttpResponseMessage response = await client.PutAsync(url, new StringContent(string.Empty));
					response.EnsureSuccessStatusCode();
					DeviceState = await response.Content.ReadAsStringAsync();

					Status = RemoteDeviceStatus.Ok;
					LatestSuccessfulAccess = DateTime.Now;
					await ExecuteExtensions();
					_stateRepository.Add(new DeviceStateMeasurement { DeviceId = Id, Time = DateTime.Now, State = State });
					_logger.LogInformation($"Action [{deviceActionDeclaration.Path}/{value}] executed on [{Name}:{Id}] on [{Address}]. New state [{DeviceState}]");
				} catch (OperationCanceledException e) {
					var msg = $"Can not execute action [{deviceActionDeclaration.Path}] on remote device [{Name}:{Id}] with address [{Address}]. Timeout expired!";
					Status = RemoteDeviceStatus.NotAccessible;
					DeviceState = null;
					_logger.LogError(e, msg);
					throw new CO2MonitorRemoteServiceException(msg, e);
				} catch (HttpRequestException e) {
					string msg = $"Can not execute action [{deviceActionDeclaration.Path}] on remote device [{Name}:{Id}] with address [{Address}]";
					Status = RemoteDeviceStatus.NotAccessible;
					DeviceState = null;
					_logger.LogError(e, msg);
					throw new CO2MonitorRemoteServiceException(msg, e);
				}
			}
		}

		public async Task<Variant> GetField(DeviceStateFieldDeclaration fieldDeclaration) {
			if (Status == RemoteDeviceStatus.NotAccessible) {
				await MakeStateRequestTask();
				if (Status == RemoteDeviceStatus.NotAccessible) {
					throw new CO2MonitorRemoteServiceException("RemoteDevice is not accessible");
				}
			}

			foreach (IDeviceExtension extension in Extensions) {
				if (extension.BaseInfo.Fields.Contains(fieldDeclaration)) {
					return await extension.GetField(fieldDeclaration);
				}
			}

			if (!BaseInfo.Fields.Contains(fieldDeclaration)) {
				throw new CO2MonitorArgumentException("RemoteDevice does not contains field " + fieldDeclaration);
			}

			Json.JObject json = Json.JObject.Parse(DeviceState ?? "{}");
			if (!json.TryGetValue(fieldDeclaration.Name, StringComparison.OrdinalIgnoreCase, out Json.JToken jToken)) {
				throw new CO2MonitorException("Can not find field " + fieldDeclaration + " in remote device state!");
			} else {
				return new Variant(fieldDeclaration.Type, jToken.ToString());
			}
		}

		private void OnSettingsChanged(string property) => 
			SettingsChanged?.Invoke(this, new PropertyChangedEventArgs(property));

		private async void MakeStateRequest(object state) => await MakeStateRequestTask();

		private async Task MakeStateRequestTask() {
			if (Address == null || (LatestSuccessfulAccess.HasValue && (DateTime.Now - LatestSuccessfulAccess.Value).TotalSeconds < 5)) {
				return;
			}

			using (var client = new HttpClient()) {
				client.Timeout = TimeSpan.FromSeconds(30);
				try {
					HttpResponseMessage response = await client.GetAsync(Address);
					response.EnsureSuccessStatusCode();
					DeviceState = await response.Content.ReadAsStringAsync();
					LatestSuccessfulAccess = DateTime.Now;
					RemoteDeviceStatus oldStatus = Status;
					Status = RemoteDeviceStatus.Ok;
					await ExecuteExtensions();
					_stateRepository.Add(new DeviceStateMeasurement { DeviceId = Id, Time = DateTime.Now, State = State });
					if (oldStatus != RemoteDeviceStatus.Ok) {
						_logger.LogInformation($"Device [{Name}:{Id}] is accessible now. Get state from [{Address}]: [{State}]");
					} else {
						_logger.LogTrace($"Get state of [{Name}:{Id}] from [{Address}]: [{State}]");
					}
				} catch (OperationCanceledException) {
					string msg = $"Can not get state of remote device [{Name}:{Id}] from [{Address}]. Timeout expired!";
					DeviceState = null;

					if (Status != RemoteDeviceStatus.NotAccessible) {
						Status = RemoteDeviceStatus.NotAccessible;
						_logger.LogError(msg);
					} else {
						_logger.LogTrace(msg);
					}
				} catch (HttpRequestException e) {
					string msg = $"Can not get state of remote device [{Name}:{Id}] from [{Address}].";
					DeviceState = null;

					if (Status != RemoteDeviceStatus.NotAccessible) {
						Status = RemoteDeviceStatus.NotAccessible;
						_logger.LogError(e, msg);
					} else {
						_logger.LogTrace(e, msg);
					}
				}
			}
		}

		private async Task ExecuteExtensions() {
			foreach (IDeviceExtension ext in Extensions) {
				try {
					await ext.Execute(this);
				} catch (CO2MonitorException ex) {
					_logger.LogError(ex, $"Device {{ Name = {Name}, Id = {Id} }} extension {{ Name = {ext.Name} }} throw exception: {ex.Message}");
				}
			}
		}

		[OnDeserialized]
		private void OnDeserialized (StreamingContext context){
			foreach (IDeviceExtension ext in Extensions) {
				ext.EventRaised += ExtensionEventRaised;
				ext.SettingsChanged += ExtensionSettingsChanged;
			}
			UpdateInfo();
		}

		private void ExtensionEventRaised(IBaseDevice sender, 
										  DeviceEventDeclaration eventDeclaration,
										  Variant data, 
										  int? senderId = null) => 
			EventRaised?.Invoke(this, eventDeclaration, data, Id);

		private void ExtensionSettingsChanged(object sender, PropertyChangedEventArgs e) => 
			SettingsChanged?.Invoke(sender, e);
	}
}
