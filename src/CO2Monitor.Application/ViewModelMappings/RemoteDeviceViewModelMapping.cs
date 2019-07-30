using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Application.ViewModels;
using AutoMapper;

namespace CO2Monitor.Application.ViewModelMappings {
	public class RemoteDeviceViewModelMapping : DeviceViewModelMappingBase<RemoteDeviceViewModel, IRemoteDevice> {
		private readonly IDeviceManagerService _deviceManager;

		public RemoteDeviceViewModelMapping(IMapper mapper, IDeviceRepository repo, IDeviceManagerService deviceManager) : base(mapper, repo) {
			_deviceManager = deviceManager;
		}

		public override RemoteDeviceViewModel Create(RemoteDeviceViewModel vm) {
			IRemoteDevice remote = _deviceManager.CreateRemoteDevice(vm.Address, vm.Name, Mapper.Map<DeviceInfoViewModel, DeviceInfo>(vm.Info));
			return Mapper.Map<IRemoteDevice, RemoteDeviceViewModel>(remote);
		}
	}
}
