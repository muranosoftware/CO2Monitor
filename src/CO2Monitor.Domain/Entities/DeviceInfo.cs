using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace CO2Monitor.Domain.Entities {
	public class DeviceInfo {
		[JsonProperty]
		public IReadOnlyCollection<DeviceStateFieldDeclaration> Fields { get; private set; }

		[JsonProperty]
		public IReadOnlyCollection<DeviceActionDeclaration> Actions { get; private set; }

		[JsonProperty]
		public IReadOnlyCollection<DeviceEventDeclaration> Events { get; private set; }

		public DeviceInfo() {
			Fields = new DeviceStateFieldDeclaration[0];
			Actions = new DeviceActionDeclaration [0];
			Events = new DeviceEventDeclaration[0];
		}

		[JsonConstructor]
		public DeviceInfo(IEnumerable<DeviceStateFieldDeclaration> fields, 
		                  IEnumerable<DeviceActionDeclaration> actions, 
		                  IEnumerable<DeviceEventDeclaration> events) {
			Fields = fields.ToArray();
			Actions = actions.ToArray();
			Events = events.ToArray();
		}
	}
}
