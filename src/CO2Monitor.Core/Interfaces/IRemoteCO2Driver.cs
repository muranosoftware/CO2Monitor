using System;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public interface IRemoteCO2Driver 
    {
        CO2Measurement GetMeasurement(string address);
    }
}
