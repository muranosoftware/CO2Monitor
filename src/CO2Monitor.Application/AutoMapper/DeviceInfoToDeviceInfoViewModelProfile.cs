using AutoMapper;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Application.AutoMapper {
	public class DeviceInfoToDeviceInfoViewModelProfile : Profile {
		public DeviceInfoToDeviceInfoViewModelProfile() {
			CreateMap<FieldViewModel, DeviceStateFieldDeclaration>();
			CreateMap<DeviceStateFieldDeclaration, FieldViewModel>();

			CreateMap<ActionViewModel, DeviceActionDeclaration>();
			CreateMap<DeviceActionDeclaration, ActionViewModel>();

			CreateMap<EventViewModel, DeviceEventDeclaration>();
			CreateMap<DeviceEventDeclaration, EventViewModel>();

			CreateMap<DeviceInfoViewModel, DeviceInfo>();
			CreateMap<DeviceInfo, DeviceInfoViewModel>();
		}
	}
}
