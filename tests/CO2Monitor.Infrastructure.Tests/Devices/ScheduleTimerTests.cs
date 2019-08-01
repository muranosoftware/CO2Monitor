using System;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using System.Threading;
using CO2Monitor.Domain.Devices;

namespace CO2Monitor.Infrastructure.Tests.Devices {
	[TestFixture]
	public class ScheduleTimerTest {

		private ScheduleTimer _timer;

		[SetUp]
		public void SetUp() {
			_timer = new ScheduleTimer();
		}

		[TearDown]
		public void DisposeTimer() {
			_timer.Dispose();
		}
		
		[Test]
		public void CheckAlarm() {
			bool alarm = false;
			_timer.EventRaised += (sender, declaration, data, id) => {
				_timer.BaseInfo.Events.Count(x => x == declaration).Should().Be(1);
				alarm = true;
			};

			_timer.AlarmTime = (DateTime.Now.AddSeconds(1) - DateTime.Today);
			Thread.Sleep(2000);
			alarm.Should().Be(true);
		}
	}
}