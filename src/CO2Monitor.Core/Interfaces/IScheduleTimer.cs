using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Interfaces
{
    public interface IScheduleTimer: IDevice
    {
        TimeSpan AlarmTime { get; set; }
    }
}
