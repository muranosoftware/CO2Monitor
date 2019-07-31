using AutoMapper;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Application.AutoMapper {
	class RemoteDeviceToRemoteDeviceViewModelProfile : Profile {
		public RemoteDeviceToRemoteDeviceViewModelProfile() {
			ShouldMapProperty = propertyInfo => true;
			ShouldUseConstructor = ctorInfo => ctorInfo.IsPrivate;

			IMappingExpression<IRemoteDevice, RemoteDeviceViewModel> map = CreateMap<IRemoteDevice, RemoteDeviceViewModel>();
			map.IncludeBase<IDevice, DeviceViewModel>();
		}
	}
}
