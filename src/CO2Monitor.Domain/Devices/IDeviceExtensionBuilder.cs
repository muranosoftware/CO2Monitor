using System;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Devices {
	public interface IDeviceExtensionBuilder {
		Type ExtensionType { get; }
		IDeviceExtension CreateDeviceExtension(string parameter, IExtendableDevice device);
	}
}
