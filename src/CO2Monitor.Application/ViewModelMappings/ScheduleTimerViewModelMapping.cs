using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Application.ViewModels;
using AutoMapper;

namespace CO2Monitor.Application.ViewModelMappings {
	public class ScheduleTimerViewModelMapping : DeviceViewModelMappingBase<ScheduleTimerViewModel, IScheduleTimer> {
		private readonly IDeviceManagerService _deviceManager;
		public ScheduleTimerViewModelMapping(IMapper mapper, IDeviceRepository repo, IDeviceManagerService deviceManager) : base(mapper, repo) {
			_deviceManager = deviceManager;
		}
		
		public override ScheduleTimerViewModel Create(ScheduleTimerViewModel vm) {
			IScheduleTimer timer = _deviceManager.CreateTimer(vm.Name, vm.AlarmTime.Value);
			return Mapper.Map<IScheduleTimer, ScheduleTimerViewModel>(timer);
		}
	}
}
