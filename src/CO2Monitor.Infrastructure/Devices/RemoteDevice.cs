using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Shared;
using System.Runtime.CompilerServices;

namespace CO2Monitor.Infrastructure.Devices
{
    public class RemoteDevice : IRemoteDevice
    {
        private readonly HashSet<DeviceStateFieldDeclaration> _stateFields = new HashSet<DeviceStateFieldDeclaration>();

        private readonly HashSet<DeviceActionDeclaration> _actions = new HashSet<DeviceActionDeclaration>();

        private readonly HashSet<DeviceEventDeclaration> _events = new HashSet<DeviceEventDeclaration>();

        private string _name;
        private float _pollingRate;
        private string _address;
        private readonly Timer _timer;
        private DeviceInfo _info;
        private readonly ILogger _logger;
        private readonly IDeviceStateRepository _stateRepository;
        
        public RemoteDevice(ILogger<RemoteDevice> logger, IDeviceStateRepository repository)
        {
            _info = new DeviceInfo(_stateFields, _actions, _events);
            _pollingRate = 60.0f;
            _timer = new Timer(MakeStateRequest, null, TimeSpan.FromSeconds(_pollingRate), TimeSpan.FromSeconds(_pollingRate));
            _logger = logger;
            _stateRepository = repository;
        }

        public int Id { get; set; }


        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    SettingsChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public bool IsRemote => true;

        public float PollingRate
        {
            get { return _pollingRate; }

            set
            {
                if (value != _pollingRate)
                {
                    _pollingRate = value;
                    OnSettingsChanged("PollingRate");
                    _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_pollingRate));
                }
            }
        }

        private IEnumerable<DeviceStateFieldDeclaration> StateFields
        {
            get
            {
                foreach (var s in _stateFields)
                    yield return s;
                foreach (var e in DeviceExtentions)
                    foreach (var s in e.Info.Fields)
                        yield return s;
            }
        }

        private IEnumerable<DeviceActionDeclaration> Actions
        {
            get
            {
                foreach (var a in _actions)
                    yield return a;
                foreach (var e in DeviceExtentions)
                    foreach (var a in e.Info.Actions)
                        yield return a;
            }
        }

        private IEnumerable<DeviceEventDeclaration> Events
        {
            get
            {
                foreach (var ev in _events)
                    yield return ev;
                foreach (var e in DeviceExtentions)
                    foreach (var ev in e.Info.Events)
                        yield return ev;
            }
        }

        public  DeviceInfo Info { get => _info; set => _info = value; }

        
        public string Address
        {
            get => _address;
            set
            {
                if (value != _address)
                {
                    _address = value;
                    OnSettingsChanged("Address");
                    MakeStateRequest(null);
                }
            }
        }

        public DateTime? LatestSuccessfullAccess { get; set; }

        public RemoteDeviceStatus Status { get; set; } = RemoteDeviceStatus.NotAccessible;

        public bool IsExtensible => true;

        public IReadOnlyCollection<IDeviceExtention> DeviceExtentions { get; set; } = new List<IDeviceExtention>();

        public string State { get; set; }

        public event PropertyChangedEventHandler SettingsChanged;
        public event DeviceEventHandler EventRaised;

        public void AddExtention(IDeviceExtention extention)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void UpdateInfo()
        {
            _info = new DeviceInfo(StateFields.ToArray(), Actions.ToArray(), Events.ToArray());
        }

        public async Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Value value)
        {
            var url = Address + "/" + deviceActionDeclaration.Path;

            if (value != null && value.Declaration.Type != ValueTypes.Void)
                url += "/" + value.ToString();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                try
                {
                    HttpResponseMessage response = await client.PutAsync(url, new StringContent(string.Empty));
                    response.EnsureSuccessStatusCode();
                    State = await response.Content.ReadAsStringAsync();

                    _stateRepository.Add(new DeviceStateMeasurement() { DeviceId = Id, Time = DateTime.Now, State = State });
                    Status = RemoteDeviceStatus.Ok;
                    LatestSuccessfullAccess = DateTime.Now;
                    _logger.LogInformation($"Action [{deviceActionDeclaration.Path}/{value?.ToString()}] executed on [{Name}:{Id}] on [{Address}]. New state [{State}]");        
                }
                catch (OperationCanceledException e)
                {
                    var msg = $"Can not execute action [{deviceActionDeclaration.Path}] on remote device [{Name}:{Id}] with adress [{Address}]. Timeout expired!";
                    Status = RemoteDeviceStatus.NotAccessible;
                    _logger.LogError(e, msg);
                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
                catch (HttpRequestException e)
                {
                    string msg = $"Can not execute action [{deviceActionDeclaration.Path}] on remote device [{Name}:{Id}] with adress [{Address}]";
                    Status = RemoteDeviceStatus.NotAccessible;
                    _logger.LogError(e, msg);
                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
            }
        }

        private void OnSettingsChanged(string property)
        {
            SettingsChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private async void MakeStateRequest(object state)
        {
            if (string.IsNullOrEmpty(Address))
                return;

            
            if (LatestSuccessfullAccess.HasValue && (DateTime.Now - LatestSuccessfullAccess.Value).TotalSeconds < 5)
                return;

            var url = Address;

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                try
                {
                    HttpResponseMessage response = await client.PutAsync(url, new StringContent(string.Empty));
                    response.EnsureSuccessStatusCode();
                    State = await response.Content.ReadAsStringAsync();
                    _stateRepository.Add(new DeviceStateMeasurement() { DeviceId = Id, Time = DateTime.Now, State = State });
                    Status = RemoteDeviceStatus.Ok;
                    LatestSuccessfullAccess = DateTime.Now;
                    _logger.LogInformation($"Get state of [{Name}:{Id}] from [{Address}]: [{State}]");

                }
                catch (OperationCanceledException)
                {
                    var msg = $"Can not get state of remote device [{Name}:{Id}] from [{Address}]. Timeout expired!";
                    Status = RemoteDeviceStatus.NotAccessible;
                    State = null;
                    _logger.LogError(msg);
                    return;
                }
                catch (HttpRequestException e)
                {
                    var msg = $"Can not get state of remote device [{Name}:{Id}] from [{Address}].";
                    Status = RemoteDeviceStatus.NotAccessible;
                    State = null;
                    _logger.LogError(e, msg);
                    return;
                }
            }
        }
    }
}
