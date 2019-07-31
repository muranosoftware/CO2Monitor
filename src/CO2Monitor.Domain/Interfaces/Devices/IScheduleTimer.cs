using System;

namespace CO2Monitor.Domain.Interfaces.Devices {
	public interface IScheduleTimer : IDevice {
		TimeSpan AlarmTime { get; set; }
	}
}
