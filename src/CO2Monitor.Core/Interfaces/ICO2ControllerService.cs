using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public enum CO2Levels
    {
        Invalid = -1,
        Low,
        Normal,
        Mid,
        High
    };

    public delegate void CO2LevelChangedHandler(ICO2ControllerService sender, CO2Levels level, CO2Measurement value);

    public interface ICO2ControllerService : IHostedService
    {
        string CO2DriverAddress { get; set; }

        void SetLevel(CO2Levels level, int value);

        int GetLevel(CO2Levels level);

        /// <summary>
        /// Seconds
        /// </summary>
        float PollingRate { get; set; }

        CO2Measurement GetLatestMeasurement();

        event CO2LevelChangedHandler CO2LevelChanged;
    }
}
