using AutoMapper;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Application.AutoMapper {
	class ScheduleTimerToScheduleTimerViewModelProfile : Profile {
		public ScheduleTimerToScheduleTimerViewModelProfile() {
			ShouldMapProperty = propertyInfo => true;

			IMappingExpression<IScheduleTimer, ScheduleTimerViewModel> map = CreateMap<IScheduleTimer, ScheduleTimerViewModel>();
			map.IncludeBase<IDevice, DeviceViewModel>();

			IMappingExpression<ScheduleTimerViewModel, IScheduleTimer> inverse = CreateMap<ScheduleTimerViewModel, IScheduleTimer>();
			inverse.IgnoreAllPropertiesWithAnInaccessibleSetter();
			inverse.ForMember(x => x.BaseInfo, opt => opt.Ignore());
			inverse.ForMember(x => x.Info, opt => opt.Ignore());
			inverse.ForMember(x => x.State, opt => opt.Ignore());
		}
	}
}
