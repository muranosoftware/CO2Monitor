using System;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public interface IDeviceExtensionBuilder {
		Type ExtensionType { get; }
		IDeviceExtension CreateDeviceExtension(string parameter, IDevice device);
	}
}
