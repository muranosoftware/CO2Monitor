using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Devices {
	public class CalendarDevice : ICalendarDevice {
		private static readonly Dictionary<DeviceStateFieldDeclaration, Func<CalendarDevice, Variant>> StateFieldDeclarations =
			new Dictionary<DeviceStateFieldDeclaration, Func<CalendarDevice, Variant>> {
				{
					new DeviceStateFieldDeclaration(nameof(IsTodayWorkDay), VariantDeclaration.BooleanEnum),
					(calendar) => new Variant(VariantDeclaration.BooleanEnum, calendar.IsTodayWorkDay.ToString())
				}, {
					new DeviceStateFieldDeclaration(nameof(IsYesterdayWorkDay), VariantDeclaration.BooleanEnum),
					(calendar) => new Variant(VariantDeclaration.BooleanEnum, calendar.IsYesterdayWorkDay.ToString())
				}, {
					new DeviceStateFieldDeclaration(nameof(IsTomorrowWorkDay), VariantDeclaration.BooleanEnum),
					(calendar) => new Variant(VariantDeclaration.BooleanEnum, calendar.IsTomorrowWorkDay.ToString())
				}
			};

		private static readonly DeviceInfo CalendarDeviceInfo = new DeviceInfo(StateFieldDeclarations.Keys, Array.Empty<DeviceActionDeclaration>(), Array.Empty<DeviceEventDeclaration>());

		// ReSharper disable once NotAccessedField.Local
		private readonly Timer _dailyTimer;
		private readonly ILogger<CalendarDevice> _logger;
		private readonly ConcurrentDictionary<DateTime, bool> _dayWorkStatuses;
		private readonly IWorkDayCalendarService _calendarService;
	
		public CalendarDevice(ILogger<CalendarDevice> logger, IWorkDayCalendarService calendarService) {
			_logger = logger;
			_dayWorkStatuses = new ConcurrentDictionary<DateTime, bool>();
			MarkSundaysAndSaturdaysAsDayOff();
			_calendarService = calendarService;
			_dailyTimer = new Timer(UpdateWorkDayStatuses, null, TimeSpan.Zero, TimeSpan.FromDays(1));
		}

		private async void UpdateWorkDayStatuses(object state) {
			ImmutableHashSet<DateTime> dates = GetDatesForUpdate();

			try {
				foreach (DateTime d in dates)
					_dayWorkStatuses[d] = await _calendarService.IsWorkDay(d);
				_dayWorkStatuses.Keys.Where(x => !dates.Contains(x)).ToList().ForEach(d =>  _dayWorkStatuses.TryRemove(d, out bool _));
			} catch (OperationCanceledException ex) {
				_logger.LogError(ex, "Can not update work day stutuses. Only saturday and sunday will be marked as day off.");
				MarkSundaysAndSaturdaysAsDayOff();
			} catch (HttpRequestException ex) {
				_logger.LogError(ex, "Can not update work day stutuses. Only saturday and sunday will be marked as day off.");
				MarkSundaysAndSaturdaysAsDayOff();
			}
		}

		private static ImmutableHashSet<DateTime> GetDatesForUpdate() {
			DateTime today = DateTime.Today;
			ImmutableHashSet<DateTime> dates = Enumerable.Range(-2, 5).Select(x => today.AddDays(x)).ToImmutableHashSet();
			return dates;
		}

		private void MarkSundaysAndSaturdaysAsDayOff() {
			ImmutableHashSet<DateTime> dates = GetDatesForUpdate();
			foreach (DateTime d in GetDatesForUpdate())
				_dayWorkStatuses[d] = d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday;
			
			_dayWorkStatuses.Keys.Where(x => !dates.Contains(x)).ToList().ForEach(d => _dayWorkStatuses.TryRemove(d, out bool _));
		}

		public void Dispose() {
			_dailyTimer.Dispose();
		}

		public string Name { get; set; } = "Calendar";

		public DeviceInfo BaseInfo => CalendarDeviceInfo;

		public DeviceInfo Info => CalendarDeviceInfo;

		event PropertyChangedEventHandler IBaseDevice.SettingsChanged { 
			add { }
			remove { }
		}

		event DeviceEventHandler IBaseDevice.EventRaised {
			add { }
			remove { }
		}
		
		public int Id { get; set; }

		public IReadOnlyCollection<IDeviceExtension> DeviceExtensions => Array.Empty<IDeviceExtension>();

		public bool IsTodayWorkDay => IsWorkDay(DateTime.Today);

		public bool IsYesterdayWorkDay => IsWorkDay(DateTime.Today.AddDays(-1));

		public bool IsTomorrowWorkDay => IsWorkDay(DateTime.Today.AddDays(1));

		public string State => JsonConvert.SerializeObject(new Dictionary<string, string> {
			{ nameof(IsTodayWorkDay), IsTodayWorkDay.ToString() },
			{ nameof(IsYesterdayWorkDay), IsYesterdayWorkDay.ToString() },
			{ nameof(IsTomorrowWorkDay), IsTomorrowWorkDay.ToString() }
		});

		public Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Variant value) {
			throw new InvalidOperationException("There is not any action in CalendarDevice");
		}

		public Task<Variant> GetField(DeviceStateFieldDeclaration fieldDeclaration) {
			if (StateFieldDeclarations.ContainsKey(fieldDeclaration))
				return Task.FromResult(StateFieldDeclarations[fieldDeclaration](this));
			else
				throw new CO2MonitorArgumentException("CalendarDevice does not contains field " + fieldDeclaration);
		}

		private bool IsWorkDay(DateTime date) {
			if (!_dayWorkStatuses.TryGetValue(date, out bool status))
				throw new InvalidOperationException("Date is too far from today");
			else
				return status;
		}
	}
}