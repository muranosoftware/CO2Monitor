using System;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.RemoteServices
{
    public class FakeRemoteCO2Driver : IRemoteCO2Driver
    {
        public CO2Measurement GetMeasurement(string address)
        {
            var rand = new Random();
            return new CO2Measurement()
            {
                CO2 = 600 + rand.Next(800),
                Temperature = 10f + 20f * (float)rand.NextDouble(),
                Time = DateTime.UtcNow
            };
        }
    }
}
