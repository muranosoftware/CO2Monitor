using System;
using System.Collections.Generic;
using System.Text;

using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Devices
{
    public interface IDeviceBuilder
    {
        Type DeviceType { get;  }

        IDevice CreateDevice();
    }
}
