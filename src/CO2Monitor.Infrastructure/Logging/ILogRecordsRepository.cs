using System;
using System.Collections.Generic;
using CO2Monitor.Core.Entities;
using System.Linq.Expressions;

namespace CO2Monitor.Infrastructure.Logging {
	public interface ILogRecordsRepository {
		void Add(LogRecord record);
		IEnumerable<LogRecord> List(Expression<Func<LogRecord, bool>> predicate = null, uint? limit = 0);
		void EnsureCreated();
	}
}
