using System;
using System.Linq.Expressions;
using System.Collections.Specialized;
using System.Collections.Generic;
using CO2Monitor.Domain.Interfaces.Devices;

namespace CO2Monitor.Domain.Interfaces.Services {
	public interface IDeviceRepository : INotifyCollectionChanged {
		IEnumerable<T> List<T>(Expression<Func<T, bool>> predicate = null) where T : class, IDevice;

		T GetById<T>(int id) where T : class, IDevice;

		T Add<T>(T device) where T : class, IDevice;

		bool Delete<T>(Expression<Func<T, bool>> predicate) where T : class, IDevice;

		void Update<T>(T device) where T : class, IDevice;
	}
}
