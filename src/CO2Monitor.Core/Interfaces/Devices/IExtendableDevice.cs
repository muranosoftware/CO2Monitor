using System.Collections.Generic;

namespace CO2Monitor.Core.Interfaces.Devices {
	public interface IExtendableDevice : IDevice {
		void AddExtension(IDeviceExtension extension);
		IEnumerable<IDeviceExtension> Extensions { get; }
	}
}
