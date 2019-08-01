using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using CO2Monitor.Infrastructure.Tests.TestHelpers;
using CO2Monitor.Domain.Devices;
using CO2Monitor.Domain.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Tests.Devices {
	[TestFixture]
	public class CalendarDeviceTests {
		[Test]
		public void CheckTodayHoliday() {
			var logger = new Mock<ILogger<CalendarDevice>>();
			logger.MockLog(LogLevel.Debug);

			var calendarSevice = new Mock<IWorkDayCalendarService>();
			calendarSevice.Setup(x => x.IsWorkDay(It.Is<DateTime>(d => d == DateTime.Today))).Returns(Task.FromResult(false));

			var calendar = new CalendarDevice(logger.Object, calendarSevice.Object);

			Thread.Sleep(500);

			calendar.IsTodayWorkDay.Should().BeFalse();
		}
	}
}