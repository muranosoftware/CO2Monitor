using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public class DeviceFactory : IDeviceFactory {
		readonly List<IDeviceBuilder> _builders;

		public DeviceFactory(IServiceProvider services) {
			_builders = services.GetServices<IDeviceBuilder>().ToList();
		}
		
		public T CreateDevice<T>() where T : IDevice {
			if (typeof(T).IsInterface)
				return (T)_builders.First(x => typeof(T).IsAssignableFrom(x.DeviceType)).CreateDevice();
			else
				return (T)_builders.First(x => x.DeviceType == typeof(T)).CreateDevice();
		}

		public IDevice CreateDevice(string name) {
			return _builders.First(x => x.DeviceType.Name == name).CreateDevice();
		}

		public IEnumerable<Type> GetDeviceTypes() {
			return _builders.Select(x => x.DeviceType);
		}
	}
}
