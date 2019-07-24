namespace CO2Monitor.Core.Interfaces.Devices {
	public interface ICalendarDevice : IDevice {
		bool IsTodayWorkDay { get; }

		bool IsYesterdayWorkDay { get; }

		bool IsTomorrowWorkDay { get; }
	}
}
