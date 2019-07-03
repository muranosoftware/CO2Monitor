using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Devices {
	public class CalendarDevice : ICalendarDevice {
		public void Dispose() {
			throw new NotImplementedException();
		}

		public string Name { get; set; }

		public DeviceInfo Info { get; set; }

		public event PropertyChangedEventHandler SettingsChanged;

		public Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Variant value) {
			throw new NotImplementedException();
		}

		public event DeviceEventHandler EventRaised;

		public int Id { get; set; }

		public bool IsRemote => false;

		public bool IsExtensible => false;
		public IReadOnlyCollection<IDeviceExtention> DeviceExtentions => Array.Empty<IDeviceExtention>();

		public void AddExtention(IDeviceExtention extention) {
			throw new NotSupportedException();
		}

		public bool IsTodayWorkDay => IsWorkDay(DateTime.Today);

		public bool IsYesterdayWorkDay => IsWorkDay(DateTime.Today.AddDays(-1));

		public bool IsTomorrowWorkDay => IsWorkDay(DateTime.Today.AddDays(1));

		public bool IsWorkDay(DateTime date) {
			throw new NotImplementedException();
		}

		public void SetDayWorkStatus(DateTime date, bool isWork) {
			throw new NotImplementedException();
		}

		public IEnumerable<DayWorkStatus> GetDayWorkStatuses(DateTime from, DateTime to) {
			throw new NotImplementedException();
		}
	}
}