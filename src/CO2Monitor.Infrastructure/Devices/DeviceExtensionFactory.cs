using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public class DeviceExtensionFactory : IDeviceExtensionFactory {
		readonly IDeviceExtensionBuilder [] _builders;

		public IDeviceExtension CreateExtension(Type type, string parameter, IExtendableDevice device) {
			return type.IsInterface
		               ? _builders.First(x => type.IsAssignableFrom(x.ExtensionType)).CreateDeviceExtension(parameter, device)
		               : _builders.First(x => x.ExtensionType == type).CreateDeviceExtension(parameter, device);
		}

        public IEnumerable<Type> GetExtensionTypes() => _builders.Select(x => x.ExtensionType);

        public DeviceExtensionFactory(IServiceProvider services) {
			_builders = services.GetServices<IDeviceExtensionBuilder>().ToArray();
		}
	}
}
