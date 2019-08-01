using System;
using System.Collections;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using CO2Monitor.Infrastructure.Services;

namespace CO2Monitor.Infrastructure.Tests.Services {
	public static class RussianHollydays {
		public static IEnumerable HollydaysDates {
			get {
				int year = DateTime.Today.Year;
				yield return new TestCaseData(new DateTime(year, 1, 01)).Returns(false);
				yield return new TestCaseData(new DateTime(year, 1, 07)).Returns(false);
				yield return new TestCaseData(new DateTime(year, 2, 23)).Returns(false);
				yield return new TestCaseData(new DateTime(year, 3, 08)).Returns(false);
				yield return new TestCaseData(new DateTime(year, 5, 01)).Returns(false);
				yield return new TestCaseData(new DateTime(year, 5, 09)).Returns(false);
				yield return new TestCaseData(new DateTime(year, 6, 12)).Returns(false);
			}
		}
	}

	public static class FirstWeekOfSeptember {
		public static IEnumerable Dates {
			get {
				int year = DateTime.Today.Year;
				for (int i = 1; i <= 7; i++) {
					DateTime date = new DateTime(year, 9, i);
					yield return new TestCaseData(date).Returns(date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday);
				}
			}
		}
	}

	[TestFixture]
	public class IsDayOffDotRuCalendarServiceTests {
		private IsDayOffDotRuCalendarService _service;

		[SetUp]
		public void Setup() {
			_service = new IsDayOffDotRuCalendarService();
		}


		[TestCaseSource(typeof(RussianHollydays), "HollydaysDates")]
		public bool CheckHolidays(DateTime date) {
			return _service.IsWorkDay(date).Result;
		}

		[TestCaseSource(typeof(FirstWeekOfSeptember), "Dates")]
		public bool CheckWeekInSeptember(DateTime date) {
			return _service.IsWorkDay(date).Result;
		}
	}
}