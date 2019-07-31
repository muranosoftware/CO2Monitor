using System;
using Microsoft.Extensions.Logging;

namespace CO2Monitor.Infrastructure.Logging {
	public class DbLoggerProvider : ILoggerProvider {
		private readonly Func<string, LogLevel, bool> _filter;
		private readonly LogRecordsRepositoryAccessor _logRecordsDbContextAccessor;

		public DbLoggerProvider(Func<string, LogLevel, bool> filter, LogRecordsRepositoryAccessor logRecordsDbContextAccessor) {
			_filter = filter;
			_logRecordsDbContextAccessor = logRecordsDbContextAccessor;
		}

		public ILogger CreateLogger(string categoryName) => new DbLogger(categoryName, _filter, _logRecordsDbContextAccessor.LogRepository);

		public void Dispose() { }
	}
}
