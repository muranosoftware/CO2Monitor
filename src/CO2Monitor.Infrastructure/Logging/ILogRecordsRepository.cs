using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Logging {
	public interface ILogRecordsRepository {
		void Add(LogRecord record);
		IEnumerable<LogRecord> List(Expression<Func<LogRecord, bool>> predicate = null, uint? limit = 0);
		void EnsureCreated();
	}
}
