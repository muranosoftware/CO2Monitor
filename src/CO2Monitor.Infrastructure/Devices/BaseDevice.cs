using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Devices
{
    public abstract class BaseDevice : IBaseDevice
    {
        public string Name { get; set; }
        public abstract DeviceInfo Info { get; set; }

        public abstract event PropertyChangedEventHandler SettingsChanged;
        public abstract event DeviceEventHandler EventRaised;

        public abstract void Dispose();
        public abstract Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Value value);
    }
}
