using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Domain.Interfaces.Devices {
	[JsonConverter(typeof(StringEnumConverter))]
	public enum RemoteDeviceStatus {
		NotAccessible,
		Ok,
	}

	public interface IRemoteDevice : IExtendableDevice {
		Uri Address { get; set; }

		float PollingRate { get; set; }

		new DeviceInfo BaseInfo { get; set; }

		DateTime? LatestSuccessfulAccess { get; }

		RemoteDeviceStatus Status { get; }

		void UpdateInfo();
	}
}
