using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;


using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Services
{
    public class CO2ControllerService : ICO2ControllerService
    {
        private const string configurationSectionKey = "CO2Controller";
        private const string configurationSettingsFileKey = "SettingsFile";
        private const string configurationCO2DriverAddressKey = "CO2DriverAddress";

        private IRemoteCO2Driver _remoteCO2Driver;

        public CO2ControllerService(IConfiguration configuration, IRemoteCO2Driver remoteCO2Driver)
        {
            _remoteCO2Driver = remoteCO2Driver;

            SettingsFilePath  = configuration.GetSection(configurationSectionKey)[configurationSettingsFileKey];

            Settings = CO2ControllerSettings.LoadOrUseDefault(SettingsFilePath);

            if (string.IsNullOrEmpty(Settings.CO2DriverAddress))
            {
                Settings.CO2DriverAddress = configuration.GetSection(configurationSectionKey)[configurationCO2DriverAddressKey];
            }
        }

        public float PollingRate
        {
            get => Settings.PollingRate;
            set
            {
                Settings.PollingRate = value;
                Settings.Save(SettingsFilePath);
            }
        }

        public string CO2DriverAddress
        {
            get => Settings.CO2DriverAddress;
            set
            {
                Settings.CO2DriverAddress = value;
                Settings.Save(SettingsFilePath);
            }
        }
        
        private string SettingsFilePath { get; }

        private CO2ControllerSettings Settings { get; }

        public int GetLevel(CO2Levels level)
        {
            return Settings.Levels[level];
        }

        public void SetLevel(CO2Levels level, int value)
        {
            Settings.Levels[level] = value;
            Settings.Save(SettingsFilePath);
        }

        public CO2Measurement GetMeasurement()
        {
            return _remoteCO2Driver.GetMeasurement(CO2DriverAddress);
        }
    }
}
