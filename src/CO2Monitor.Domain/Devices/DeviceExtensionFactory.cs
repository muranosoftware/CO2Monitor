using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Devices {
	public class DeviceExtensionFactory : IDeviceExtensionFactory {
		readonly IDeviceExtensionBuilder[] _builders;

		public IDeviceExtension CreateExtension(Type type, string parameter, IExtendableDevice device) {
			IDeviceExtensionBuilder builder = type.IsInterface ? _builders.First(x => type.IsAssignableFrom(x.ExtensionType)) :
			                                                     _builders.First(x => x.ExtensionType == type);
			return builder.CreateDeviceExtension(parameter, device);
		}

		public IEnumerable<Type> GetExtensionTypes() => _builders.Select(x => x.ExtensionType);

		public DeviceExtensionFactory(IServiceProvider services) {
			_builders = services.GetServices<IDeviceExtensionBuilder>().ToArray();
		}
	}
}
