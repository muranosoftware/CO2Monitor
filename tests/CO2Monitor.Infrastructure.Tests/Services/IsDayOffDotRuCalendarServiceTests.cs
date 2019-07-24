using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using CO2Monitor.Infrastructure.Services;

namespace CO2Monitor.Infrastructure.Tests.Services {
	[TestFixture]
	public class IsDayOffDotRuCalendarServiceTests {
		[Test]
		public async Task CheckHolidays() {
			int year = DateTime.Today.Year;
			var holidays = new[] {
				new DateTime(year, 1, 1),
				new DateTime(year, 1, 7),
				new DateTime(year, 2, 23),
				new DateTime(year, 3, 8),
				new DateTime(year, 5, 1),
				new DateTime(year, 5, 9),
				new DateTime(year, 6, 12),
			};

			var service = new IsDayOffDotRuCalendarService();
			
			foreach (DateTime d in holidays) {
				var isWork = await service.IsWorkDay(d);
				isWork.Should().BeFalse();
			}
		}

		[Test]
		public async Task CheckWeek() {
			int year = DateTime.Today.Year;
			
			var service = new IsDayOffDotRuCalendarService();

			for (int i = 0; i < 7; i++) {
				var date = new DateTime(year, 9, 1 + i);
				var isWork = await service.IsWorkDay(date);
				isWork.Should().Be(date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday);
			}
		}
	}
}