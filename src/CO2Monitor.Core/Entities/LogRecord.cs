using System;

namespace CO2Monitor.Core.Entities {
	public class LogRecord {
		public int Id { get; set; }
		public int? EventId { get; set; }
		public string LogLevel { get; set; }
		public string Message { get; set; }
		public DateTime Time { get; set; }
	}
}
