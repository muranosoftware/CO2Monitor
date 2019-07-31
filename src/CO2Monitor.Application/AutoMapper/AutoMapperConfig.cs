using AutoMapper;

namespace CO2Monitor.Application.AutoMapper {
	public class AutoMapperConfig {
		public static MapperConfiguration RegisterMappings() {
			return new MapperConfiguration(cfg => {
				cfg.AddProfile(new DeviceToDeviceViewModelProfile());
				cfg.AddProfile(new ScheduleTimerToScheduleTimerViewModelProfile());
				cfg.AddProfile(new RemoteDeviceToRemoteDeviceViewModelProfile());
				cfg.AddProfile(new DeviceInfoToDeviceInfoViewModelProfile());
				cfg.AddProfile(new ActionRuleToRuleViewModelProfile());
			});
		}
	}
}
