using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Domain.Interfaces.Devices {
	public interface IDevice : IBaseDevice {
		int Id { get; set; }

		DeviceInfo Info { get; } 
	}
}
