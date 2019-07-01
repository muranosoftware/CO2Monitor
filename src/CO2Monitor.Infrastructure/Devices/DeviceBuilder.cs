using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Devices
{
    class DeviceBuilder<T> : IDeviceBuilder where T : IDevice
    {
        private readonly IServiceProvider _serviceProvider;

        public DeviceBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Type DeviceType => typeof(T);

        public IDevice CreateDevice()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
