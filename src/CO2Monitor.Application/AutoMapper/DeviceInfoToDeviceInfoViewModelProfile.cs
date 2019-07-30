using AutoMapper;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Application.AutoMapper {
	public class DeviceInfoToDeviceInfoViewModelProfile : Profile {
		public DeviceInfoToDeviceInfoViewModelProfile() {
			CreateMap<FieldViewModel, DeviceStateFieldDeclaration>();
			CreateMap<ActionViewModel, DeviceActionDeclaration>();
			CreateMap<EventViewModel, DeviceEventDeclaration>();
			CreateMap<DeviceInfoViewModel, DeviceInfo>();
		}
	}
}
