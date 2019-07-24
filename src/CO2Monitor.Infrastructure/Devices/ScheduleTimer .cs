using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public class ScheduleTimer : IScheduleTimer {
		private static readonly IReadOnlyDictionary<DeviceStateFieldDeclaration, Func<ScheduleTimer, Variant>> StateFieldDeclarations = new Dictionary<DeviceStateFieldDeclaration, Func<ScheduleTimer, Variant>> {
			{ new DeviceStateFieldDeclaration(nameof(AlarmTime), VariantDeclaration.Time), (timer) => new Variant(timer.AlarmTime) }
		};

		private static readonly IReadOnlyDictionary<DeviceActionDeclaration, Func<ScheduleTimer, Variant, Task>> Actions = new Dictionary<DeviceActionDeclaration, Func<ScheduleTimer, Variant, Task>> {
			{
				new DeviceActionDeclaration("SetAlarmTime", new VariantDeclaration(VariantType.Time)), (timer, val) => {
					if (val.Declaration.Type != VariantType.Time)
						throw new InvalidOperationException();
					timer.AlarmTime = val.Time;
					return Task.CompletedTask;
				}
			},
		};

		private static readonly DeviceEventDeclaration AlarmEventDeclaration = new DeviceEventDeclaration("Alarm", new VariantDeclaration(VariantType.Time));

		private static readonly DeviceEventDeclaration[] EventDeclarations = new DeviceEventDeclaration[] {
			AlarmEventDeclaration
		};

		private static readonly DeviceInfo TimerDeviceInfo = new DeviceInfo(StateFieldDeclarations.Keys, Actions.Keys, EventDeclarations);

		TimeSpan _alarmTime;
		Timer _timer;
		private string _name = nameof(ScheduleTimer);
		
		public ScheduleTimer() {
			_timer = new Timer(Alarm);
			AlarmTime = TimeSpan.FromHours(12);
			UpdateInternalTimer();
		}

		public string Name {
			get => _name;
			set {
				if (_name != value) {
					_name = value;
					SettingsChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
				}
			}
		}

		public int Id { get; set; }

		public DeviceInfo BaseInfo => TimerDeviceInfo;

		public DeviceInfo Info => TimerDeviceInfo;

		public TimeSpan AlarmTime {
			get => _alarmTime;
			set {
				if (value.TotalHours < 0 || value.TotalHours > 24.0)
					throw new ArgumentException("AlarmTime must be greater than zero and less than 24 hours");

				if (value != _alarmTime) {
					_alarmTime = value;
					UpdateInternalTimer();
					OnSettingsChanged("AlarmTime");
				}
			}
		}

		public string State => JsonConvert.SerializeObject(new Dictionary<string, string> {
			{ nameof(AlarmTime), AlarmTime.ToString() }
		});

		public event PropertyChangedEventHandler SettingsChanged;

		public event DeviceEventHandler EventRaised;

		public Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Variant value) {
			if (!Actions.ContainsKey(deviceActionDeclaration))
				throw new InvalidOperationException();

			return Actions[deviceActionDeclaration](this, value);
		}

		public Task<Variant> GetField(DeviceStateFieldDeclaration fieldDeclaration) {
			if (StateFieldDeclarations.ContainsKey(fieldDeclaration))
				return Task.FromResult(StateFieldDeclarations[fieldDeclaration](this));
			else
				throw new CO2MonitorArgumentException("ScheduleTimer does not contains field " + fieldDeclaration);
		}

		public void Dispose() {
			_timer.Dispose();
		}

		private void OnSettingsChanged(string property) {
			SettingsChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		private void Alarm(object state) {
			OnAlarmEventRaised();
			UpdateInternalTimer();
		}

		private void OnAlarmEventRaised() {
			EventRaised?.Invoke(this, AlarmEventDeclaration, new Variant(AlarmTime), Id);
		}

		private void UpdateInternalTimer() {
			_timer.Dispose();

			TimeSpan dt = DateTime.Today - DateTime.Now + AlarmTime;

			if (dt.TotalHours < 0)
				dt = dt.Add(TimeSpan.FromDays(1));

			if (dt.TotalMinutes < 0) {
				if (dt.TotalMinutes < -1)
					throw new Exception("Something very bad");
				dt = TimeSpan.Zero; 
			}

			_timer = new Timer(Alarm, null, dt, TimeSpan.FromDays(1));
		}
	}
}
