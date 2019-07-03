using System;

namespace CO2Monitor.Core.Interfaces {
	public interface IScheduleTimer : IDevice {
		TimeSpan AlarmTime { get; set; }
	}
}
