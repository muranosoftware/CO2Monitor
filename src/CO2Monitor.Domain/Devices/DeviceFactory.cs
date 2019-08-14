using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Devices {
	public class DeviceFactory : IDeviceFactory {
		readonly List<IDeviceBuilder> _builders;

		public DeviceFactory(IServiceProvider services) {
			_builders = services.GetServices<IDeviceBuilder>().ToList();
		}
		
		public T CreateDevice<T>() where T : IDevice {
			return typeof(T).IsInterface
				? (T)_builders.Single(x => typeof(T).IsAssignableFrom(x.DeviceType)).CreateDevice()
				: (T)_builders.Single(x => x.DeviceType == typeof(T)).CreateDevice();
		}

		public IDevice CreateDevice(string name) => _builders.Single(x => x.DeviceType.Name == name).CreateDevice();

		public IEnumerable<Type> GetDeviceTypes() => _builders.Select(x => x.DeviceType);
	}
}
