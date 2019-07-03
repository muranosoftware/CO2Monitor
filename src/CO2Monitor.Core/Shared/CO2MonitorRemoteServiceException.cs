using System;

namespace CO2Monitor.Core.Shared {
	public class CO2MonitorRemoteServiceException : CO2MonitorException {
		public CO2MonitorRemoteServiceException() { }

		public CO2MonitorRemoteServiceException(string message) : base(message) { }

		public CO2MonitorRemoteServiceException(string message, Exception innerException) : base(message, innerException) { }
	}
}
