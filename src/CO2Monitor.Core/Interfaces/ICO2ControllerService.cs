using System;
using System.Collections.Generic;
using System.Text;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public enum CO2Levels
    {
        Normal,
        Mid,
        High
    };

    public interface ICO2ControllerService
    {
        string CO2DriverAddress { get; set; }

        void SetLevel(CO2Levels level, int value);

        int GetLevel(CO2Levels level);

        /// <summary>
        /// Seconds
        /// </summary>
        float PollingRate { get; set; }

        CO2Measurement GetMeasurement();
    }
}
