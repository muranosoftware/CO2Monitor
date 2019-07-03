using System;
using System.Collections.Generic;

namespace CO2Monitor.Core.Interfaces {
	public class DayWorkStatus {
		public DayWorkStatus(DateTime date, bool isWork) {
			Date = date;
			IsWork = isWork;
		}

		public DateTime Date { get; }

		public bool IsWork { get; }
	}
	
	interface ICalendarDevice : IDevice {
		bool IsTodayWorkDay { get; }

		bool IsYesterdayWorkDay { get; }

		bool IsTomorrowWorkDay { get; }

		bool IsWorkDay(DateTime date);

		void SetDayWorkStatus(DateTime date, bool isWork);

		IEnumerable<DayWorkStatus> GetDayWorkStatuses(DateTime from, DateTime to);
	}
}
