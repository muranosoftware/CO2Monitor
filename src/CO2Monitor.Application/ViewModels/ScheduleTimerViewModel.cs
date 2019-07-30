using System;

namespace CO2Monitor.Application.ViewModels {
	public class ScheduleTimerViewModel : DeviceViewModel {
		public ScheduleTimerViewModel (int id, string name, TimeSpan? alarmTime)  : 
			base(id, name, "ScheduleTimer", null, false, false, null) {
			AlarmTime = alarmTime;
		}

		public TimeSpan? AlarmTime { get; private set; }

		private ScheduleTimerViewModel() { } // for AutoMapper
	}
}
