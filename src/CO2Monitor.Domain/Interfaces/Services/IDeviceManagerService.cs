using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using CO2Monitor.Domain.Interfaces.Devices;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Domain.Interfaces.Services {
	public interface IDeviceManagerService : IHostedService {
		IDeviceRepository DeviceRepository { get; }

		IActionRuleRepository RuleRepository { get; }

		IScheduleTimer CreateTimer(string name, TimeSpan time);

		IRemoteDevice CreateRemoteDevice(Uri address, string name, DeviceInfo deviceInfo);

		IDeviceExtension CreateDeviceExtension(Type type, int deviceId, string parameter);

		IEnumerable<Type> GetDeviceExtensionsTypes();

		Task ExecuteAction(int deviceId, string action, string argument);
	}
}
