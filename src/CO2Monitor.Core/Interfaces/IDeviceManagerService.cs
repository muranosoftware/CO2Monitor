using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public interface IDeviceManagerService : IHostedService
    {
        IDeviceRepository DeviceRepository { get; }

        IActionRuleRepository RuleRepository { get; }

        IScheduleTimer CreateTimer(string name, TimeSpan time);

        IRemoteDevice CreateRemoteDevice(string address, string name, DeviceInfo deviceInfo);

        void ExecuteAction(int deviceId, string action, string argument);
    }
}
