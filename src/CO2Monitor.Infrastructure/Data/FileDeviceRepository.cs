using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MoreLinq;
using CO2Monitor.Infrastructure.Helpers;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Data {
	public class FileDeviceRepository : IDeviceRepository {
		class DeviceData {
			public int DeviceIdSeq { get; set; }

			public IDictionary<int, IDevice> Devices { get; set; } = new ConcurrentDictionary<int, IDevice>();

			[MethodImpl(MethodImplOptions.Synchronized)]
			public int GetNextId() => ++DeviceIdSeq;
		}

		private const string ConfigurationFileKey = "FileDeviceRepository:File";
		private readonly string _fileName;
		private readonly JsonSerializerSettings _jsonSettings;
		private readonly ILogger<FileDeviceRepository> _logger;
		private readonly DeviceData _data;

		public FileDeviceRepository(ILogger<FileDeviceRepository> logger, 
									IConfiguration configuration, 
									IDeviceFactory deviceFactory,
									IServiceProvider serviceProvider) {
			_logger = logger;
			_fileName = configuration.GetValue<string>(ConfigurationFileKey);

			var jsonResolver = new PropRenAndIgnDepInjSerializerContractResolver(serviceProvider);
			foreach (Type t in deviceFactory.GetDeviceTypes()) {
				jsonResolver.IgnoreProperty(typeof(IBaseDevice), nameof(IBaseDevice.State));
				jsonResolver.IgnoreProperty(typeof(IDevice), nameof(IDevice.Info));

				if (t.GetInterfaces().Contains(typeof(IDevice))) {
					jsonResolver.IgnoreProperty(t,
					                            nameof(IDevice.Info));
				}
				
				if (t.GetInterfaces().Contains(typeof(ICalendarDevice))) {
					jsonResolver.IgnoreProperty(t,
					                            nameof(ICalendarDevice.IsTodayWorkDay), 
					                            nameof(ICalendarDevice.IsTomorrowWorkDay), 
					                            nameof(ICalendarDevice.BaseInfo), 
					                            nameof(ICalendarDevice.IsYesterdayWorkDay));
				}

				if (t.GetInterfaces().Contains(typeof(IScheduleTimer))) {
					jsonResolver.IgnoreProperty(t,nameof(IScheduleTimer.BaseInfo));
				}

				if (t.GetInterfaces().Contains(typeof(IRemoteDevice))) {
					jsonResolver.IgnoreProperty(t,
					                            nameof(IRemoteDevice.LatestSuccessfulAccess), 
					                            nameof(IRemoteDevice.Status));
				}
			}

			_jsonSettings = new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
				SerializationBinder = new TypeBinder(),
				ContractResolver = jsonResolver,
			};

			if (File.Exists(_fileName)) {
				_data = JsonConvert.DeserializeObject<DeviceData>(File.ReadAllText(_fileName), _jsonSettings);
				_data.Devices.Values.ForEach(d => d.SettingsChanged += DeviceSettingsChanged);
			} else {
				_data = new DeviceData();
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public T Add<T>(T device) where T : class, IDevice {
			if (_data.Devices.ContainsKey(device.Id)) {
				return device;
			}

			device.Id = _data.GetNextId();
			_logger.LogInformation($"Adding device [{device.GetType().Name}:{device.Name}:{device.Id}] to repo");

			_data.Devices.Add(device.Id, device);
			device.SettingsChanged += DeviceSettingsChanged;

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, device));
			Save();

			return device;
		}

		public bool Delete<T>(Expression<Func<T, bool>> predicate) where T : class, IDevice {
			Func<T, bool> func = predicate.Compile();

			T[] devices = _data.Devices.Values.OfType<T>().Where(func).ToArray();
			
			foreach (T d in devices) {
				_logger.LogInformation($"Deleting device [{d.GetType().Name}:{d.Name}:{d.Id}] from repo");
				d.SettingsChanged -= DeviceSettingsChanged;
				_data.Devices.Remove(d.Id);
				d.Dispose();
			}

			Save();

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, devices));

			return devices.Any();
		}

		public IEnumerable<T> List<T>(Expression<Func<T, bool>> predicate = null) where T : class, IDevice {
			if (predicate is null) {
				return _data.Devices.Values.OfType<T>();
			}

			Func<T, bool> func = predicate.Compile();
			return _data.Devices.Values.OfType<T>().Where(func);
		}

		public void Update<T>(T device) where T : class, IDevice => Save();

		private void DeviceSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => Save();

		private void Save() {
			_logger.LogInformation($"Saving device configuration to file [{_fileName}]");
			var json = JsonConvert.SerializeObject(_data, _jsonSettings);
			File.WriteAllText(_fileName, json);
		}

		public T GetById<T>(int id) where T : class, IDevice => _data.Devices.ContainsKey(id) ? (T)_data.Devices[id] : null;
	}
}
