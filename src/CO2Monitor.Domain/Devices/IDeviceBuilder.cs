using System;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Devices {
	public interface IDeviceBuilder {
		Type DeviceType { get; }

		IDevice CreateDevice();
	}
}
