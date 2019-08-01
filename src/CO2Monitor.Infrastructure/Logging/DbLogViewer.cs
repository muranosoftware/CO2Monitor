using System;
using System.Collections.Generic;
using CO2Monitor.Infrastructure.Helpers;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Logging {
	public class DbLogViewer : ILogViewer {
		private readonly ILogRecordsRepository _repo;

		public DbLogViewer(ILogRecordsRepository repo) {
			_repo = repo;
		}

		public IEnumerable<LogRecord> GetRecords(DateTime? from = null, DateTime? to = null, uint? limit = 100) {
			var predicateBuilder = new PredicateBuilder<LogRecord>();

			if (from != null) {
				predicateBuilder.AndAlso(x => x.Time >= from.Value);
			}

			if (to != null) {
				predicateBuilder.AndAlso(x => x.Time >= to.Value);
			}

			IEnumerable<LogRecord> result = predicateBuilder.IsEmpty ? _repo.List(null, limit) : _repo.List(predicateBuilder.Predicate, limit);

			return result;
		}
	}
}
