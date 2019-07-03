using System;

namespace CO2Monitor.Core.Shared {
	public class CO2MonitorArgumentException : CO2MonitorException {
		public CO2MonitorArgumentException() { }

		public CO2MonitorArgumentException(string argument) : base("Ivalid argument: " + argument) {
			Argument = argument;
		}

		public CO2MonitorArgumentException(string argument, Exception innerException) : base("Ivalid argument: " + argument, innerException) {
			Argument = argument;
		}

		public CO2MonitorArgumentException(string argument, string message) : base(message) {
			Argument = argument;
		}

		public CO2MonitorArgumentException(string argument, string message, Exception innerException) : base(message, innerException) {
			Argument = argument;
		}

		public string Argument { get; }
	}
}
