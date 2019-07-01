using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using System.Threading;

namespace CO2Monitor.Infrastructure.Devices
{
    public class ScheduleTimer : IScheduleTimer
    {
        private static readonly DeviceStateFieldDeclaration[] _StateFieldDeclarations = new DeviceStateFieldDeclaration[]
        {
            new DeviceStateFieldDeclaration(nameof(AlarmTime), new ValueDeclaration(ValueTypes.Time))
        };

        private static readonly IReadOnlyDictionary<DeviceActionDeclaration, Func<ScheduleTimer, Value, Task>> _Actions =  new Dictionary<DeviceActionDeclaration, Func<ScheduleTimer, Value, Task>> ()
        {
            {
                new DeviceActionDeclaration("SetAlarmTime", new ValueDeclaration(ValueTypes.Time)), (timer, val) => 
                {
                    if (val.Declaration.Type != ValueTypes.Time)
                        throw new InvalidOperationException();
                    timer.AlarmTime = val.Time;
                    return Task.CompletedTask;
                }
            },
            
        };

        private static readonly DeviceEventDeclaration _AlarmEventDeclaration = new DeviceEventDeclaration("Alarm", new ValueDeclaration(ValueTypes.Time));

        private static readonly DeviceEventDeclaration[] _EventDeclarations = new DeviceEventDeclaration[]
        {
            _AlarmEventDeclaration
        };

        private static readonly IReadOnlyCollection<IDeviceExtention> _DeviceExtentions = Array.Empty<IDeviceExtention>();

        private static readonly DeviceInfo _Info = new DeviceInfo(_StateFieldDeclarations, _Actions.Keys.ToArray(), _EventDeclarations);

        TimeSpan _alarmTime;
        Timer _timer;

        public ScheduleTimer()
        {
            _timer = new Timer(Alarm);
            AlarmTime = TimeSpan.FromHours(12);
            UpdateInernalTimer();
        }

        public string Name { get; set; } = nameof(ScheduleTimer);
        public int Id { get; set; }

        public bool IsRemote => false;

        public bool IsExtensible => false;

        public DeviceInfo Info { get => _Info; set { } }

        public IReadOnlyCollection<IDeviceExtention> DeviceExtentions => _DeviceExtentions;

        public TimeSpan AlarmTime
        {
            get { return _alarmTime; }
            set
            {
                if (value.TotalHours < 0 || value.TotalHours > 24.0)
                    throw new ArgumentException("AlarmTime must be greater than zero and less than 24 hours");

                if (value != _alarmTime)
                {
                    _alarmTime = value;
                    UpdateInernalTimer();
                    OnSettingsChanged("AlarmTime");
                }
            }
        }
        public event PropertyChangedEventHandler SettingsChanged;

        public event DeviceEventHandler EventRaised;

        public void AddExtention(IDeviceExtention extention)
        {
            throw new NotSupportedException();
        }

        public Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Value value)
        {
            if(!_Actions.ContainsKey(deviceActionDeclaration))
                throw new InvalidOperationException();

            return _Actions[deviceActionDeclaration](this, value);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private void OnSettingsChanged(string property)
        {
            SettingsChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Alarm(object state)
        {
            OnAlarmEventRaised();
            UpdateInernalTimer();
        }

        private void OnAlarmEventRaised()
        {
            EventRaised?.Invoke(this, _AlarmEventDeclaration, new Value(AlarmTime), Id);
        }

        private void UpdateInernalTimer()
        {
            _timer.Dispose();

            TimeSpan dt =  DateTime.Today - DateTime.Now + AlarmTime;

            if (dt.TotalHours < 0)
                dt = dt.Add(TimeSpan.FromDays(1));

            if (dt.TotalMinutes < 0)
            {
                if (dt.TotalMinutes < -1)
                    throw new Exception("Something very bad");
                dt = TimeSpan.Zero; 
            }


            _timer = new Timer(Alarm, null, dt, TimeSpan.FromDays(1));
        }
    }
}
