using System.Linq;
using AutoMapper;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Application.AutoMapper {
	public class DeviceToDeviceViewModelProfile : Profile {
		public DeviceToDeviceViewModelProfile() {
			ShouldMapProperty = propertyInfo => true;

			IMappingExpression<IDevice, DeviceViewModel> map = CreateMap<IDevice, DeviceViewModel>();

			map.ForMember(x => x.IsExtendable, opt => opt.MapFrom(src => src is IExtendableDevice));
			map.ForMember(x => x.IsRemote, opt => opt.MapFrom(src => src is IRemoteDevice));
			map.ForMember(x => x.Type, opt => opt.MapFrom(src => src.GetType().Name));
			map.ForMember(x => x.Info, opt => opt.MapFrom(src =>
				new DeviceInfoViewModel(src.Info.Fields.Select(x => new FieldViewModel(x.Name, x.Type, !src.BaseInfo.Fields.Contains(x))).ToArray(),
				                        src.Info.Actions.Select(x => new ActionViewModel(x.Path, x.Argument, !src.BaseInfo.Actions.Contains(x))).ToArray(),
				                        src.Info.Events.Select(x => new EventViewModel(x.Name, x.DataType, !src.BaseInfo.Events.Contains(x))).ToArray())));
		}
	}
}
