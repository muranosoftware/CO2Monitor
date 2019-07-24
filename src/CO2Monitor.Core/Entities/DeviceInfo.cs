using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace CO2Monitor.Core.Entities {
	public class DeviceInfo {
		[JsonProperty]
		public IReadOnlyCollection<DeviceStateFieldDeclaration> Fields {
			get;
			private set;
		}

		[JsonProperty]
		public IReadOnlyCollection<DeviceActionDeclaration> Actions { get; private set; }

		[JsonProperty]
		public IReadOnlyCollection<DeviceEventDeclaration> Events { get; private set; }

		public DeviceInfo() {
			Fields = new List<DeviceStateFieldDeclaration>().AsReadOnly();
			Actions = new List<DeviceActionDeclaration>().AsReadOnly();
			Events = new List<DeviceEventDeclaration>().AsReadOnly();
		}

		public DeviceInfo(IEnumerable<DeviceStateFieldDeclaration> fields, 
		                  IEnumerable<DeviceActionDeclaration> actions, 
		                  IEnumerable<DeviceEventDeclaration> events) {
			Fields = fields.ToList().AsReadOnly();
			Actions = actions.ToList().AsReadOnly();
			Events = events.ToList().AsReadOnly();
		}
	}
}
