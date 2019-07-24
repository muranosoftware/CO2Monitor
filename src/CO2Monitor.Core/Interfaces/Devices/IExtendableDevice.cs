namespace CO2Monitor.Core.Interfaces.Devices
{
	public interface IExtendableDevice : IDevice {
		void AddExtension(IDeviceExtension extension);
	}
}
