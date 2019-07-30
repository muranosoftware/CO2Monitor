using System;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Logging {
	public class DbLogger : ILogger {
		private const int MessageMaxLength = 4000;

		private readonly string _categoryName;
		private readonly Func<string, LogLevel, bool> _filter;
		private readonly ILogRecordsRepository _repo;

		public DbLogger(string categoryName, Func<string, LogLevel, bool> filter, ILogRecordsRepository repo) {
			_categoryName = categoryName;
			_filter = filter;
			_repo = repo;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
			if (!IsEnabled(logLevel)) {
				return;
			}

			if (formatter == null) {
				throw new ArgumentNullException(nameof(formatter));
			}

			var message = formatter(state, exception);

			if (string.IsNullOrEmpty(message)) {
				return;
			}

			if (exception != null) {
				message += "\n" + exception;
			}

			message = message.Length > MessageMaxLength ? message.Substring(0, MessageMaxLength) : message;
			var log = new LogRecord {
				Message = message,
				EventId = eventId.Id,
				LogLevel = logLevel.ToString(),
				Time = DateTime.Now
			};

			_repo.Add(log);
		}

		public bool IsEnabled(LogLevel logLevel) => (_filter == null || _filter(_categoryName, logLevel));

		public IDisposable BeginScope<TState>(TState state) => null;
	}
}
