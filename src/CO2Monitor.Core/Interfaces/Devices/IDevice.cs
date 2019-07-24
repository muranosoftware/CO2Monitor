using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces.Devices {
	public interface IDevice : IBaseDevice {
		int Id { get; set; }

		DeviceInfo Info { get; } 
	}
}
