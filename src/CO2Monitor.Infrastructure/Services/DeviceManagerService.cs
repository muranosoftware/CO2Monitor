using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Entities;
using System.Collections.Generic;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Services {
	public class DeviceManagerService : IDeviceManagerService {
		private readonly ILogger _logger;
		private readonly IActionRuleRepository _ruleRepository;
		private readonly IDeviceRepository _deviceRepository;
		private readonly IDeviceFactory _deviceFactory;
		private readonly IDeviceExtensionFactory _extensionFactory;
		private readonly IEventNotificationService _notificationService;

		public DeviceManagerService(ILogger<DeviceManagerService> logger, 
		                            IActionRuleRepository ruleRepository, 
		                            IDeviceRepository deviceRepository, 
		                            IDeviceFactory deviceFactory, 
		                            IDeviceExtensionFactory extensionFactory, 
		                            IEventNotificationService notificationService) {
			_ruleRepository = ruleRepository;
			_deviceRepository = deviceRepository;
			_deviceFactory = deviceFactory;
			_extensionFactory = extensionFactory;
			_notificationService = notificationService;
			_logger = logger;

			if (_deviceRepository.List<ICalendarDevice>().FirstOrDefault() == null) {
				var calendar = _deviceFactory.CreateDevice<ICalendarDevice>();
				calendar.EventRaised += DeviceEventRaised;
				_deviceRepository.Add(calendar);
			}

			foreach (IDevice d in _deviceRepository.List<IDevice>()) {
				d.EventRaised += DeviceEventRaised;
			}
		}

		public IDeviceRepository DeviceRepository => _deviceRepository;

		public IActionRuleRepository RuleRepository => _ruleRepository;

		public async Task ExecuteAction(int deviceId, string action, string argument) {
			var device = _deviceRepository.GetById<IDevice>(deviceId);

			if (device == null)
				throw new CO2MonitorArgumentException(nameof(deviceId), $"There is not device with id = {0}");

			DeviceActionDeclaration act = device.Info.Actions.FirstOrDefault(x => x.Path == action);

			if (act == null)
				throw new CO2MonitorArgumentException(nameof(deviceId), $"Device[{deviceId}] does not have action [{action}]");

			var val = new Variant(act.Argument, argument);

			await device.ExecuteAction(act, val);
		}

		public IScheduleTimer CreateTimer(string name, TimeSpan time) {
			var timer = _deviceFactory.CreateDevice<IScheduleTimer>();
			timer.EventRaised += DeviceEventRaised;
			timer.AlarmTime = time;
			timer.Name = name;
			return _deviceRepository.Add(timer);
		}

		public IRemoteDevice CreateRemoteDevice(Uri address, string name, DeviceInfo deviceInfo) {
			var remote = _deviceFactory.CreateDevice<IRemoteDevice>();
			remote.EventRaised += DeviceEventRaised;
			remote.Name = name;
			remote.Address = address;
			remote.BaseInfo = deviceInfo;
			return _deviceRepository.Add(remote);
		}

		private async void DeviceEventRaised(IBaseDevice sender, DeviceEventDeclaration eventDeclaration, Variant data, int? deviceId) {
			_logger.LogInformation($"Event [{eventDeclaration.Name}] from [{sender.Name}:{deviceId ?? -1}] raised with data [{data}].");

			foreach (ActionRule r in _ruleRepository.List(x => x.SourceDeviceId == deviceId)) {
				_logger.LogInformation($"Found rule [{r.Name}:{r.Id}] binded to event [{eventDeclaration.Name}] from [{sender.Name}:{deviceId ?? -1}]");
				var device = _deviceRepository.GetById<IDevice>(r.TargetDeviceId);
				if (device == null) {
					_logger.LogInformation($"Target device [{r.TargetDeviceId}] of rule [{r.Name}:{r.Id}] not found. Skip");
					continue;
				}

				if (!device.Info.Actions.Contains(r.Action)) {
					_logger.LogInformation($"Target device [{r.TargetDeviceId}] of rule [{r.Name}:{r.Id}] does not have action [{r.Action}]. Skip");
					continue;
				}

				bool breaked = false;
				foreach (ActionCondition cond in r.Conditions) {
					try {
						if (!await CheckCondition(cond)) {
							_logger.LogInformation($"Condition [{cond}] is violated. Rule [{r.Name}:{r.Id}] is skipped.");
							breaked = true;
							break;
						}
					} catch (CO2MonitorException ex) {
						_logger.LogError(ex, $"Can not check condition [{cond}]. Rule [{r.Name}:{r.Id}] is skipped.");
						breaked = true;
						break;
					}
				}

				if (breaked)
					continue;

				_logger.LogInformation($"Try execute rule's [{r.Name}:{r.Id}] action on [{r.TargetDeviceId}]");
				Variant argument;

				switch (r.ArgumentSource) {
					case RuleActionArgumentSource.Constant:
						try {
							argument = new Variant(r.Action.Argument, r.ActionArgument);
						} catch (CO2MonitorArgumentException ex) {
							_logger.LogError(ex, $"Can not convert rule's argument constant [{r.ActionArgument}] to action argument type [{r.Action.Argument}]. Rule skip");
							continue;
						}
						break;
					case RuleActionArgumentSource.EventData:
						if (data.Declaration != r.Action.Argument) {
							_logger.LogError($"Event [{eventDeclaration.Name}] data has incompatible type [{data.Declaration}] with action [{r.Action.Path}] argument [{r.Action.Argument}]. Rule Skip");
							continue;
						}
						argument = data;
						break;
					default:
						throw new NotImplementedException();
				}
				try {
					await device.ExecuteAction(r.Action, argument);
				} catch (CO2MonitorException ex) {
					_logger.LogError(ex, $"Can not execute rule [{r.Name}.{r.Id}] action  {device.Name}{{ Id = {device.Id}}}{r.Action}.");
				}
				var message = $"{device.Name}{{ Id = {device.Id}}}.{r.Action.Path}({argument.String}) executed using rule [{r.Name}.{r.Id}]";
				await _notificationService.Notify(message);
			}
		}
		
		public async Task<bool> CheckCondition(ActionCondition condition) {
			var device = _deviceRepository.GetById<IDevice>(condition.DeviceId);

			if (device is null) {
				_logger.LogError($"Can not found device with id [{condition.DeviceId}]. Condition [{condition}] is violated.");
				return false;
			}

			if (!device.Info.Fields.Contains(condition.Field)) {
				_logger.LogError($"Device [{device.Name}:{device.Id}] has not field [{condition.Field}]. Condition [{condition}] is violated.");
				return false;
			}

			Variant fieldValue = await device.GetField(condition.Field);
			var condVal = new Variant(condition.Field.Type, condition.ConditionArgument);

			switch (condition.ConditionType) {
				case ConditionType.Equal:
					return fieldValue == condVal;
				case ConditionType.NotEqual:
					return fieldValue != condVal;
				case ConditionType.LessThan:
					return fieldValue < condVal;
				case ConditionType.GreaterThan:
					return fieldValue > condVal;
				case ConditionType.LessThanOrEqual:
					return fieldValue <= condVal;
				case ConditionType.GreaterThanOrEqual:
					return fieldValue >= condVal;
				default:
					throw new NotImplementedException();
			}
		}

		public IDeviceExtension CreateDeviceExtension(Type type, int deviceId, string parameter){
			var device = _deviceRepository.GetById<IDevice>(deviceId);
			if (device == null)
				throw new CO2MonitorArgumentException($"Can not find device with id {deviceId}");
			if (!(device is IExtendableDevice))
				throw new CO2MonitorArgumentException($"Device (id ={deviceId}) is not extensible!");
			var extDevice = device as IExtendableDevice;
			IDeviceExtension ext = (_extensionFactory.CreateExtension(type, parameter, device));
			extDevice.AddExtension(ext);
			return ext;
		}

		public IEnumerable<Type> GetDeviceExtensionsTypes(){
			return _extensionFactory.GetExtensionTypes();
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			_logger.LogInformation("Starting DeviceManagerService background service.");
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			_logger.LogInformation("Stopping DeviceManagerService background service.");

			foreach (IDevice d in _deviceRepository.List<IDevice>())
				d.Dispose();

			return Task.CompletedTask;
		}
	}
}
