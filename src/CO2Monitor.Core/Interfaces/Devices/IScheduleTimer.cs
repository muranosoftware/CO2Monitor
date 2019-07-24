using System;

namespace CO2Monitor.Core.Interfaces.Devices {
	public interface IScheduleTimer : IDevice {
		TimeSpan AlarmTime { get; set; }
	}
}
