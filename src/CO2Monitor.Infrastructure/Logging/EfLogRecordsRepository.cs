using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using CO2Monitor.Domain.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Logging {
	public class EfLogRecordsRepository : ILogRecordsRepository {
		private readonly LogRecordsDbContext _dbContext;

		private readonly object _lock = new object();

		public EfLogRecordsRepository(LogRecordsDbContext dbContext) {
			_dbContext = dbContext;
		}

		public void Add(LogRecord record) {
			lock (_lock) { 
				_dbContext.Records.Add(record);
				_dbContext.SaveChanges();
			}
		}

		public void EnsureCreated() {
			lock (_lock) {
				_dbContext.Database.EnsureCreated();
			}
		}

		public IEnumerable<LogRecord> List(Expression<Func<LogRecord, bool>> predicate = null, uint? limit = 0) {
			lock (_lock) {
				return predicate != null
					? _dbContext.Records.Where(predicate).OrderByDescending(x => x.Time).ToList()
					: _dbContext.Records.OrderByDescending(x => x.Time).ToList();
			}
		}
	}
}
