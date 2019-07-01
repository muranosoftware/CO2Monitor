﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using CO2Monitor.Infrastructure.Helpers;
using CO2Monitor.Infrastructure.Devices;

namespace CO2Monitor.Infrastructure.Services
{
    public class DeviceManagerService : IDeviceManagerService
    {
       
        

        private readonly ILogger _logger;
        private readonly IActionRuleRepository _ruleRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceFactory _deviceFactory;

        public DeviceManagerService(ILogger<DeviceManagerService> logger, 
                                    IActionRuleRepository ruleRepository,
                                    IDeviceRepository deviceRepository,
                                    IDeviceFactory deviceFactory)
        {
            _ruleRepository = ruleRepository;
            _deviceRepository = deviceRepository;
            _deviceFactory = deviceFactory;
            _logger = logger;
        }

        public IDeviceRepository DeviceRepository => _deviceRepository;

        public IActionRuleRepository RuleRepository => _ruleRepository;

        public void ExecuteAction(int deviceId, string action, string argument)
        {
            var device = _deviceRepository.GetById<IDevice>(deviceId);

            if(device == null)
                throw new CO2MonitorArgumentException(nameof(deviceId), $"There is not device with id = {0}");

            DeviceActionDeclaration act = device.Info.Actions.FirstOrDefault(x => x.Path == action);

            if(act == null)
                throw new CO2MonitorArgumentException(nameof(deviceId), $"Device[{deviceId}] does not have action [{action}]");

            var val = new Value(act.Argument, argument);

            device.ExecuteAction(act, val);
        }

        public IScheduleTimer CreateTimer(string name, TimeSpan time)
        {
            var timer = _deviceFactory.CreateDevice<IScheduleTimer>();
            timer.EventRaised += DeviceEventRaised;
            timer.AlarmTime = time;
            timer.Name = name;
            return _deviceRepository.Add(timer);
        }

        public IRemoteDevice CreateRemoteDevice(string address, string name, DeviceInfo deviceInfo)
        {
            var remote = _deviceFactory.CreateDevice<IRemoteDevice>();
            remote.Name = name;
            remote.Address = address;
            remote.Info = deviceInfo;
            return _deviceRepository.Add(remote);
        }

        private void DeviceEventRaised(IBaseDevice sender, DeviceEventDeclaration eventDeclaration, Value data, int? deviceId)
        {
            _logger.LogInformation($"Event [{eventDeclaration.Name}] from [{sender.Name}:{deviceId??-1}] raised with data: {data}.");

            foreach (var r in _ruleRepository.List( x=> x.SourceDeviceId == deviceId))
            {
                var device = _deviceRepository.GetById<IDevice>(r.TargetDeviceId);
                if (device == null)
                {
                    _logger.LogInformation($"Target device [{r.TargetDeviceId}] of rule [{r.Name}:{r.Id}] not found. Skip");
                    continue;
                }

                if (device.Info.Actions.Contains(r.Action))
                    device.ExecuteAction(r.Action, new Value(r.Action.Argument, r.ActionArgument));
                else
                    _logger.LogInformation($"Target device [{r.TargetDeviceId}] of rule [{r.Name}:{r.Id}] does not have action [{r.Action}]. Skip");
            }
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting DeviceManagerService background service.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping DeviceManagerService background service.");

            foreach (var d in _deviceRepository.List<IDevice>())
                d.Dispose();

            return Task.CompletedTask;
        }

    }
}
