using System;

namespace CO2Monitor.Core.Shared {
	public class CO2MonitorConflictException : CO2MonitorException {
		public CO2MonitorConflictException() { }

		public CO2MonitorConflictException(string message) : base(message) { }

		public CO2MonitorConflictException(string message, Exception innerException) : base(message, innerException) { }
	}
}
