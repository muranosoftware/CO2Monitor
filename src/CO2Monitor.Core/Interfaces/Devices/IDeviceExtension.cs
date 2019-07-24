using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces.Devices {
	public interface IDeviceExtension : IBaseDevice {
		Task Execute(IDevice device);
	}
}
