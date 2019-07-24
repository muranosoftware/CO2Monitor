using System;
using System.Collections.Generic;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces.Services {
	public interface ILogViewer {
		IEnumerable<LogRecord> GetRecords(DateTime? from = null, DateTime? to = null, uint? limit = 1000);
	}
}
