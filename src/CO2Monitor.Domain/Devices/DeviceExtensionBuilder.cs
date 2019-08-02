﻿using System;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Devices {
	public class DeviceExtensionBuilder<T> : IDeviceExtensionBuilder where T : IDeviceExtension {
		public Type ExtensionType => typeof(T);

		private readonly Func<string, IExtendableDevice, T> _activator;

		public IDeviceExtension CreateDeviceExtension(string parameter, IExtendableDevice device) =>
			_activator(parameter, device);

		public DeviceExtensionBuilder(Func<string, IExtendableDevice, T> activator) {
			_activator = activator;
		}
	}
}