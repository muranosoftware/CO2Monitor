using System;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public interface IDeviceBuilder {
		Type DeviceType { get; }

		IDevice CreateDevice();
	}
}
