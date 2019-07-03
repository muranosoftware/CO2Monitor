using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CO2Monitor.Core.Interfaces {
	[JsonConverter(typeof(StringEnumConverter))]
	public enum RemoteDeviceStatus {
		NotAccessible,
		Ok,
	}

	public interface IRemoteDevice : IDevice {
		string Address { get; set; }

		float PollingRate { get; set; }

		DateTime? LatestSuccessfulAccess { get; }

		RemoteDeviceStatus Status { get; }

		string State { get; }

		void UpdateInfo();
	}
}
