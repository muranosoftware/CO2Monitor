using System;
using System.Collections.Generic;
using System.Text;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public interface IDeviceStateRepository
    {
        void Add(DeviceStateMeasurement measurement);

        IEnumerable<DeviceStateMeasurement> List(int deviceId, DateTime? from = null, DateTime? to = null);
    }
}
