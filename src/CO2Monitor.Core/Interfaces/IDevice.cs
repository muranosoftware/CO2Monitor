using System.Collections.Generic;

namespace CO2Monitor.Core.Interfaces {
	public interface IDevice : IBaseDevice {
		int Id { get; set; }

		bool IsRemote { get; }

		bool IsExtensible { get; }

		IReadOnlyCollection<IDeviceExtention> DeviceExtentions { get; }

		void AddExtention(IDeviceExtention extention);
	}
}
