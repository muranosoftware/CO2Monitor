using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using CO2Monitor.Core.Interfaces;
using CO2Monitor.Infrastructure.Helpers;
using System.Collections.Specialized;

namespace CO2Monitor.Infrastructure.Data
{
    public class FileDeviceRepository : IDeviceRepository
    {
        class DeviceData
        {
            public int DeviceIdSeq { get; set; }

            public IDictionary<int, IDevice> Devices { get; set; } = new ConcurrentDictionary<int, IDevice>();

            [MethodImpl(MethodImplOptions.Synchronized)]
            public int GetNextId() { return ++DeviceIdSeq; }
          
        }

        private const string _ConfigurationFile = "Devices.json";
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly ILogger<FileDeviceRepository> _logger;


        public FileDeviceRepository(IConfiguration configuration,
                                    ILogger<FileDeviceRepository> logger,
                                    IDeviceFactory deviceFactory,
                                    IServiceProvider serviceProvider)
        {
            _logger = logger;

            var jsonResolver = new PropRenAndIgnDepInjSerializerContractResolver(serviceProvider);
            foreach (var t in deviceFactory.GetDeviceTypes())
                jsonResolver.IgnoreProperty(t,
                    nameof(IDevice.IsRemote),
                    nameof(IDevice.IsExtensible),
                    nameof(IRemoteDevice.LatestSuccessfullAccess),
                    nameof(IRemoteDevice.Status),
                    nameof(IRemoteDevice.State));

            _jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new TypeBinder(),
                ContractResolver = jsonResolver,
            };

            if (File.Exists(_ConfigurationFile))
                _data = JsonConvert.DeserializeObject<DeviceData>(File.ReadAllText(_ConfigurationFile), _jsonSettings);
            else
                _data = new DeviceData();
        }

        readonly DeviceData _data;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public T Add<T>(T device) where T : class, IDevice
        {
            device.Id = _data.GetNextId();
            _logger.LogInformation($"Adding device [{device.GetType().Name}:{device.Name}:{device.Id}] to repo");

            _data.Devices.Add(device.Id, device);
            device.SettingsChanged += DeviceSettingsChanged;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, device));
            Save();

            return device;
        }

        public bool Delete<T>(Predicate<T> condition) where T : class, IDevice
        {
            var found = false;
            var devices = _data.Devices.Values.OfType<T>().Where((x) => condition(x)).ToList();
            if (devices.Count > 0)
                found = true;

            foreach (var d in devices)
            {
                _logger.LogInformation($"Deleting device [{d.GetType().Name}:{d.Name}:{d.Id}] from repo");
                d.SettingsChanged -= DeviceSettingsChanged;
                _data.Devices.Remove(d.Id);
                d.Dispose();
            }

            Save();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, devices));

            return found;
        }

        public IEnumerable<T> List<T>(Predicate<T> condition = null) where T : class, IDevice
        {
            if (condition != null)
                return _data.Devices.Values.OfType<T>().Where(x => condition(x));
            else
                return _data.Devices.Values.OfType<T>();
        }

        public void Update<T>(T device) where T : class, IDevice
        {
            Save();
        }

        private void DeviceSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            _logger.LogInformation($"Saving device configuration to file [{_ConfigurationFile}]");
            var json = JsonConvert.SerializeObject(_data, _jsonSettings);
            File.WriteAllText(_ConfigurationFile, json);
        }

        public T GetById<T>(int id) where T : class, IDevice
        {
            if (!_data.Devices.ContainsKey(id))
                return null;
            else
                return (T)_data.Devices[id];

        }
    }
}
