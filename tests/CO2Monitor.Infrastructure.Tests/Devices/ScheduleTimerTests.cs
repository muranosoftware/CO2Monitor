using System;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using CO2Monitor.Infrastructure.Devices;
using System.Threading;

namespace CO2Monitor.Infrastructure.Tests.Devices {
	[TestFixture]
	public class ScheduleTimerTest {
		[Test]
		public void Alarm() {
			bool alarm = false;
			var timer = new ScheduleTimer();
			timer.EventRaised += (sender, declaration, data, id) => {
				timer.BaseInfo.Events.Count(x => x == declaration).Should().Be(1);
				alarm = true;
			};

			timer.AlarmTime = (DateTime.Now.AddSeconds(1) - DateTime.Today);
			Thread.Sleep(2000);
			alarm.Should().Be(true);
		}
	}
}