using AutoMapper;
using CO2Monitor.Domain.Entities;
using CO2Monitor.Domain.Interfaces.Devices;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Application.ViewModels;

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
