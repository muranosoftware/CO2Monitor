using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Data {
	public class EfDeviceStateRepository : IDeviceStateRepository {
		private readonly StateMeasurementDbContext _dbContext;

		public EfDeviceStateRepository(StateMeasurementDbContext dbContext) {
			_dbContext = dbContext; 
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Add(DeviceStateMeasurement measurement) {
			_dbContext.Measurements.Add(measurement);

			try {
				_dbContext.SaveChanges();
			} catch (Exception ex) {
				Console.WriteLine($"DbLoggerDbContext Error: {ex.Message}");
			}
		}

		public IEnumerable<DeviceStateMeasurement> List(Expression<Func<DeviceStateMeasurement, bool>> predicate = null) {
			return predicate == null
				? _dbContext.Measurements.OrderBy(x => x.Time)
				: _dbContext.Measurements.Where(predicate).OrderBy(x => x.Time);
		}

		public void EnsureCreated() => _dbContext.Database.EnsureCreated();
	}
}
