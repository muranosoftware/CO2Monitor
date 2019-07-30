using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CO2Monitor.Application.ViewModels {
	[JsonConverter(typeof(StringEnumConverter))]
	public enum RemoteDeviceStatus {
		NotAccessible,
		Ok,
	}

	public class RemoteDeviceViewModel : DeviceViewModel {
		public RemoteDeviceViewModel(int id, 
		                             string name, 
		                             Uri address,
		                             DeviceInfoViewModel info,
		                             string state, RemoteDeviceStatus status) :
			base(id, name, "RemoteDevice", info, true, true, state) {
			Address = address;
			Status = status;
		}

		public Uri Address { get; private set; }

		public RemoteDeviceStatus Status { get; private set; }

		private RemoteDeviceViewModel() { } // for AutoMapper

	}
}
