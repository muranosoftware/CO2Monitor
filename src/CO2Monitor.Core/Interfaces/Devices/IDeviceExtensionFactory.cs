﻿using System;
using System.Collections.Generic;

namespace CO2Monitor.Core.Interfaces.Devices {
	public interface IDeviceExtensionFactory {
		IEnumerable<Type> GetExtensionTypes();

		IDeviceExtension CreateExtension(Type type, string parameter, IDevice device);
	}
}