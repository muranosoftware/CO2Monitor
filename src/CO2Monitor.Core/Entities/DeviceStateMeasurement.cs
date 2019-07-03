using System;

namespace CO2Monitor.Core.Entities {
	public class DeviceStateMeasurement {
		public int Id { get; set; }

		public int DeviceId { get; set; }

		public DateTime Time { get; set; }

		/// <summary>
		/// json
		/// </summary>
		public string State { get; set; }
	}
}
