using System;
using System.Threading.Tasks;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public interface IRemoteCO2Driver 
    {
        Task<CO2Measurement> GetMeasurement(string address);
    }
}
