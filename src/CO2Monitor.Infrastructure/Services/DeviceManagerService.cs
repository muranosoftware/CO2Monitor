using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Entities;
using System.Collections.Generic;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Services {
	public class DeviceManagerService: IDeviceManagerService {
		private readonly ILogger _logger;
		private readonly IDeviceFactory _deviceFactory;
		private readonly IDeviceExtensionFactory _extensionFactory;
		private readonly IEventNotificationService _notificationService;

		public DeviceManagerService(ILogger<DeviceManagerService> logger,
									IActionRuleRepository ruleRepository,
									IDeviceRepository deviceRepository,
									IDeviceFactory deviceFactory,
									IDeviceExtensionFactory extensionFactory,
									IEventNotificationService notificationService) {
			RuleRepository = ruleRepository;
			DeviceRepository = deviceRepository;
			_deviceFactory = deviceFactory;
			_extensionFactory = extensionFactory;
			_notificationService = notificationService;
			_logger = logger;

			if (DeviceRepository.List<ICalendarDevice>().FirstOrDefault() == null) {
				var calendar = _deviceFactory.CreateDevice<ICalendarDevice>();
				calendar.EventRaised += DeviceEventRaised;
				DeviceRepository.Add(calendar);
			}

			foreach (IDevice d in DeviceRepository.List<IDevice>()) {
				d.EventRaised += DeviceEventRaised;
			}
		}

		public IDeviceRepository DeviceRepository { get; }

		public IActionRuleRepository RuleRepository { get; }

		public async Task ExecuteAction(int deviceId, string action, string argument) {
			var device = DeviceRepository.GetById<IDevice>(deviceId);

			if (device == null) {
				throw new CO2MonitorArgumentException(nameof(deviceId), $"There is not device with id = {0}");
			}

			DeviceActionDeclaration act = device.Info.Actions.FirstOrDefault(x => x.Path == action);

			if (act == null) {
				throw new CO2MonitorArgumentException(nameof(deviceId), $"Device[{deviceId}] does not have action [{action}]");
			}

			var val = new Variant(act.Argument, argument);

			await device.ExecuteAction(act, val);
		}

		public IScheduleTimer CreateTimer(string name, TimeSpan time) {
			var timer = _deviceFactory.CreateDevice<IScheduleTimer>();
			timer.EventRaised += DeviceEventRaised;
			timer.AlarmTime = time;
			timer.Name = name;
			return DeviceRepository.Add(timer);
		}

		public IRemoteDevice CreateRemoteDevice(Uri address, string name, DeviceInfo deviceInfo) {
			var remote = _deviceFactory.CreateDevice<IRemoteDevice>();
			remote.EventRaised += DeviceEventRaised;
			remote.Name = name;
			remote.Address = address;
			remote.BaseInfo = deviceInfo;
			return DeviceRepository.Add(remote);
		}

		private async void DeviceEventRaised(IBaseDevice sender, DeviceEventDeclaration eventDeclaration, Variant data, int? deviceId) {
			_logger.LogInformation($"Event [{eventDeclaration.Name}] from [{sender.Name}:{deviceId ?? -1}] raised with data [{data}].");

			foreach (ActionRule r in RuleRepository.List(x => x.SourceDeviceId == deviceId)) {
				_logger.LogInformation($"Found rule [{r.Name}:{r.Id}] binded to event [{eventDeclaration.Name}] from [{sender.Name}:{deviceId ?? -1}]");
				var device = DeviceRepository.GetById<IDevice>(r.TargetDeviceId);
				if (device == null) {
					_logger.LogInformation($"Target device [{r.TargetDeviceId}] of rule [{r.Name}:{r.Id}] not found. Skip");
					continue;
				}

				if (!device.Info.Actions.Contains(r.Action)) {
					_logger.LogInformation($"Target device [{r.TargetDeviceId}] of rule [{r.Name}:{r.Id}] does not have action [{r.Action}]. Skip");
					continue;
				}

				if( !(await CheckConditions(r))) {
					continue;
				}

				_logger.LogInformation($"Try execute rule's [{r.Name}:{r.Id}] action on [{r.TargetDeviceId}]");

				if (!TryGetRuleActionArgument(r, eventDeclaration, data, out Variant argument)) {
					continue;
				}

				try {
					await device.ExecuteAction(r.Action, argument);
				} catch (CO2MonitorException ex) {
					_logger.LogError(ex, $"Can not execute rule [{r.Name}.{r.Id}] action  {device.Name}{{ Id = {device.Id}}}{r.Action}.");
				}
				var message = $"{device.Name}{{ Id = {device.Id}}}.{r.Action.Path}({argument.String}) executed using rule [{r.Name}.{r.Id}]";
				_notificationService.Notify(message);
			}
		}

		private bool TryGetRuleActionArgument(ActionRule rule, DeviceEventDeclaration eventDeclaration, Variant eventData, out Variant argument) {
			argument = null;
			switch (rule.ArgumentSource) {
				case RuleActionArgumentSource.Constant:
					try {
						argument = new Variant(rule.Action.Argument, rule.ActionArgument);
					} catch (CO2MonitorArgumentException ex) {
						_logger.LogError(ex, $"Can not convert rule's argument constant [{rule.ActionArgument}] to action argument type [{rule.Action.Argument}].  Rule [{rule.Name}:{rule.Id}] is skipped.");
						return false;
					}
					break;
				case RuleActionArgumentSource.EventData:
					if (eventData.Declaration != rule.Action.Argument) {
						_logger.LogError($"Event [{eventDeclaration.Name}] data has incompatible type [{eventData.Declaration}] with action [{rule.Action.Path}] argument [{rule.Action.Argument}]. Rule [{rule.Name}:{rule.Id}] is skipped.");
						return false;
					}
					argument = eventData;
					break;
				default:
					throw new NotImplementedException();
			}
			return true;
		}

		private async Task<bool> CheckConditions(ActionRule rule) {
			foreach (ActionCondition cond in rule.Conditions) {
				try {
					if (!await CheckCondition(cond)) {
						_logger.LogInformation($"Condition [{cond}] is violated. Rule [{rule.Name}:{rule.Id}] is skipped.");
						return false;
					}
				} catch (CO2MonitorException ex) {
					_logger.LogError(ex, $"Can not check condition [{cond}]. Rule [{rule.Name}:{rule.Id}] is skipped.");
					return false;
				}
			}

			return true;
		}

		public async Task<bool> CheckCondition(ActionCondition condition) {
			var device = DeviceRepository.GetById<IDevice>(condition.DeviceId);

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
			var device = DeviceRepository.GetById<IExtendableDevice>(deviceId);
			if (device == null) {
				throw new CO2MonitorArgumentException($"Can not find extendable device with id {deviceId}");
			}

			var extDevice = device as IExtendableDevice;
			IDeviceExtension ext = (_extensionFactory.CreateExtension(type, parameter, device));
			extDevice.AddExtension(ext);
			return ext;
		}

		public IEnumerable<Type> GetDeviceExtensionsTypes() => _extensionFactory.GetExtensionTypes();

		public Task StartAsync(CancellationToken cancellationToken) {
			_logger.LogInformation("Starting DeviceManagerService background service.");
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			_logger.LogInformation("Stopping DeviceManagerService background service.");

			DeviceRepository.List<IDevice>().ForEach(d => d.Dispose());
			

			return Task.CompletedTask;
		}
	}
}
