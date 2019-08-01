using System;
using System.Collections.Generic;

namespace CO2Monitor.Infrastructure.Interfaces {
	public class LogRecord {
		public int Id { get; set; }
		public int? EventId { get; set; }
		public string LogLevel { get; set; }
		public string Message { get; set; }
		public DateTime Time { get; set; }
	}

	public interface ILogViewer {
		IEnumerable<LogRecord> GetRecords(DateTime? from = null, DateTime? to = null, uint? limit = 1000);
	}
}
