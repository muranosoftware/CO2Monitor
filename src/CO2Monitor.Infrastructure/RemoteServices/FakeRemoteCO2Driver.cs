using System;
using System.Threading.Tasks;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.RemoteServices
{
    public class FakeRemoteCO2Driver : IRemoteCO2Driver
    {
        public async Task<CO2Measurement> GetMeasurement(string address)
        {
            var rand = new Random();
            return await Task<CO2Measurement>.Run(() => new CO2Measurement
            {
                CO2 = 600 + rand.Next(800),
                Temperature = 10f + 20f * (float)rand.NextDouble(),
                Time = DateTime.UtcNow
            });
        }
    }
}
