using System.Threading.Tasks;

namespace CO2Monitor.Domain.Interfaces.Devices {
	public interface IDeviceExtension : IBaseDevice {
		Task Execute(IDevice device);
	}
}
