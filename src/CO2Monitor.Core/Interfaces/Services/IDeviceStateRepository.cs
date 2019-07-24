﻿using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces.Services {
	public interface IDeviceStateRepository {
		void Add(DeviceStateMeasurement measurement);

		IEnumerable<DeviceStateMeasurement> List(Expression<Func<DeviceStateMeasurement, bool>> predicate = null);

		void EnsureCreated();
	}
}
