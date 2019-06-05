using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;

namespace CO2Monitor.Infrastructure.Services
{
    public class CO2ControllerService : ICO2ControllerService, IDisposable
    {
        private const string configurationSectionKey = "CO2Controller";
        private const string configurationSettingsFileKey = "SettingsFile";
        private const string configurationCO2DriverAddressKey = "CO2DriverAddress";
        private const string configurationCO2FanControllerAddressKey = "CO2FanControllerAddress";

        private readonly IRemoteCO2Driver _remoteCO2Driver;
        private readonly IRemoteCO2FanController _remoteCO2FanController;
        private readonly IMeasurementRepository _repository;
        private readonly ILogger _logger;
        private CO2Measurement _latestMeasurement;
        private Timer _timer;

        public event CO2LevelChangedHandler CO2LevelChanged;

        public CO2ControllerService(IConfiguration configuration,
                                    IServiceScopeFactory serviceScopeFactory, 
                                    IRemoteCO2Driver remoteCO2Driver, 
                                    IRemoteCO2FanController remoteCO2FanController,
                                    ILogger<CO2ControllerService> logger)
        {
            _logger = logger;

            _remoteCO2Driver = remoteCO2Driver;

            _remoteCO2FanController = remoteCO2FanController;

            CO2LevelChanged += SwitchFanOnCO2LevelChanged;

            using (var scope = serviceScopeFactory.CreateScope())
            {
                _repository = scope.ServiceProvider.GetService<IMeasurementRepository>();
            }

            SettingsFilePath = configuration.GetSection(configurationSectionKey)[configurationSettingsFileKey];

            Settings = CO2ControllerSettings.LoadOrDefault(SettingsFilePath);
            ConfigurateRemoteAdresses(configuration);
        }

        private void SwitchFanOnCO2LevelChanged(ICO2ControllerService sender, CO2Levels level, CO2Measurement value)
        {
            var command = level == CO2Levels.High ? FanCommand.On : FanCommand.Off;

            var led = FanLed.Green;
            if (level == CO2Levels.Mid)
                led = FanLed.Yellow;
            if (level == CO2Levels.High)
                led = FanLed.Red;



            //if (_remoteCO2FanController.GetCommand(CO2FanDriverAddress) != command)
            //{
            try
            {
                _logger.LogInformation($"Fan [{CO2FanDriverAddress}] set command {command}");
                _remoteCO2FanController.SetCommamd(CO2FanDriverAddress, command);
            }
            catch (CO2MonitorRemoteServiceException)
            {

            }
            //}

            try
            {
                _logger.LogInformation($"Led [{CO2FanDriverAddress}] set {led}");
                _remoteCO2FanController.SetLed(CO2FanDriverAddress, led);
            }
            catch (CO2MonitorRemoteServiceException)
            {

            }
        }

        private void ConfigurateRemoteAdresses(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(Settings.CO2DriverAddress))
            {
                _logger.LogInformation($"Can not find CO2DriverAddress in saved settings. Try find CO2DriverAddress in configuration.");
                Settings.CO2DriverAddress = configuration.GetSection(configurationSectionKey)[configurationCO2DriverAddressKey];
                Settings.Save(SettingsFilePath);
            }
            else
            {
                _logger.LogInformation($"Using CO2DriverAddress from saved settings.");
            }

            _logger.LogInformation($"CO2DriverAddress: " + Settings.CO2DriverAddress);


            if (string.IsNullOrEmpty(Settings.CO2FanDriverAddress))
            {
                _logger.LogInformation($"Can not find CO2FanDriverAddress in saved settings. Try find CO2FanDriverAddress in configuration.");
                Settings.CO2FanDriverAddress = configuration.GetSection(configurationSectionKey)[configurationCO2FanControllerAddressKey];
                Settings.Save(SettingsFilePath);
            }
            else
            {
                _logger.LogInformation($"Using CO2FanDriverAddress from saved settings.");
            }

            _logger.LogInformation($"CO2FanDriverAddress: " + Settings.CO2FanDriverAddress);
        }

        public float PollingRate
        {
            get => Settings.PollingRate;
            set
            {
                if (value <= float.Epsilon)
                {
                    throw new CO2MonitorArgumentException("PollingRate", "PollingRate must be greater than zero");
                }

                _logger.LogInformation("New PollingRate: " + value);

                Settings.PollingRate = value;
                Settings.Save(SettingsFilePath);

                _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Settings.PollingRate));
            }
        }

        public string CO2DriverAddress
        {
            get => Settings.CO2DriverAddress;
            set
            {
                if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out _))
                {
                    throw new CO2MonitorArgumentException("CO2DriverAddress", $"Invalid address format: {value}");
                }

                _logger.LogInformation("New CO2DriverAddress: " + value);

                Settings.CO2DriverAddress = value;
                Settings.Save(SettingsFilePath);

                MakeMeasurement(null);
            }
        }

        public string CO2FanDriverAddress
        {
            get => Settings.CO2FanDriverAddress;
            set
            {
                if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out _))
                {
                    throw new CO2MonitorArgumentException("CO2DriverAddress", $"Invalid address format: {value}");
                }

                _logger.LogInformation("New CO2FanDriverAddress: " + value);

                Settings.CO2FanDriverAddress = value;
                Settings.Save(SettingsFilePath);

                MakeMeasurement(null);
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
            if (level == CO2Levels.Low && Settings.Levels[CO2Levels.Low + 1] <= value)
                throw new CO2MonitorArgumentException("value", $"Normal level must be less {CO2Levels.Low + 1} level");

            if (level == CO2Levels.High && Settings.Levels[CO2Levels.High - 1] >= value)
                throw new CO2MonitorArgumentException("value", $"High level must be greater {CO2Levels.High - 1} level");

            if (CO2Levels.Low < level && level < CO2Levels.High &&
                !(Settings.Levels[level - 1] < value && value < Settings.Levels[level + 1]))
                throw new CO2MonitorArgumentException("value", $"{level} level must be between {level - 1} and {level + 1} levels.");

            _logger.LogInformation($"New {level} level value: " + value);

            Settings.Levels[level] = value;
            Settings.Save(SettingsFilePath);
        }

        public async Task<CO2Measurement> GetLatestMeasurement()
        {
            if (_latestMeasurement != null)
                return _latestMeasurement;
            return await _remoteCO2Driver.GetMeasurement(CO2DriverAddress);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting CO2Controller background service.");

            _timer = new Timer(MakeMeasurement, null, TimeSpan.Zero, TimeSpan.FromSeconds(Settings.PollingRate));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping CO2Controller background service.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private void OnCO2LevelChanged(CO2Levels level, CO2Measurement value)
        {
            if (CO2LevelChanged != null)
                CO2LevelChanged(this, level, value);
        }

        private async void MakeMeasurement(object state)
        {
            try
            {
                var measurement = await _remoteCO2Driver.GetMeasurement(Settings.CO2DriverAddress);

                _repository.Add(measurement);

                CO2Levels prevLevel = _latestMeasurement == null ? CO2Levels.Invalid : GetMeasurementLevel(_latestMeasurement);
                CO2Levels curLevel = GetMeasurementLevel(measurement);
                _latestMeasurement = measurement;

                if (prevLevel != curLevel)
                {
                    _logger.LogInformation("CO2 Level changed to : " + curLevel);
                    OnCO2LevelChanged(curLevel, _latestMeasurement);
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, $"Cancelation operation!");
            }
            catch (CO2MonitorRemoteServiceException ex)
            {
                _logger.LogError(ex, $"Can not get measurment!");
            }
        }

        private CO2Levels GetMeasurementLevel(CO2Measurement measurement)
        {
            var intervals = Settings.Levels.OrderBy(x => x.Key)
                                            .Select(x => (double)x.Value)
                                            .Append(double.PositiveInfinity).ToList();
      
            CO2Levels level = CO2Levels.Invalid;
            for (int i = 0; i < intervals.Count - 1; i++)
            {
                if (intervals[i] < measurement.CO2 && measurement.CO2 <= intervals[i + 1])
                    level = (CO2Levels)i;
            }

            return level;
        }
    }
}
