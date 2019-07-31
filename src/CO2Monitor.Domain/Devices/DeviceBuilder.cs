using System;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Devices {
	class DeviceBuilder<T> : IDeviceBuilder where T : IDevice {
		private readonly IServiceProvider _serviceProvider;

		public DeviceBuilder(IServiceProvider serviceProvider) {
			_serviceProvider = serviceProvider;
		}

		public Type DeviceType => typeof(T);

		public IDevice CreateDevice() => _serviceProvider.GetService<T>();
	}
}
