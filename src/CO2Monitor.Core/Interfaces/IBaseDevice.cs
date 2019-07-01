using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using CO2Monitor.Core.Entities;
using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces
{
    public delegate void DeviceEventHandler(IBaseDevice sender, DeviceEventDeclaration eventDeclaration, Value data, int? senderId = null);

    public interface IBaseDevice : IDisposable
    {
        string Name { get; set; }

        DeviceInfo Info { get; set;  } 
        
        event PropertyChangedEventHandler SettingsChanged;

        Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Value value);

        event DeviceEventHandler EventRaised;
    }
}
