﻿using System;
using System.Collections.Generic;

namespace CO2Monitor.Core.Interfaces.Devices {
	public interface IDeviceFactory {
		IEnumerable<Type> GetDeviceTypes();

		IDevice CreateDevice(string name);

		T CreateDevice<T>() where T : IDevice;
	}
}