using System.Collections.Generic;
using System.Linq;

namespace CO2Monitor.Core.Entities {
	public class DeviceInfo {
		//public IReadOnlyCollection<DeviceStateFieldDeclaration> Fields { get; private set; }
		//
		//public IReadOnlyCollection<DeviceActionDeclaration> Actions { get; private set; }
		//
		//public IReadOnlyCollection<DeviceEventDeclaration> Events { get; private set; }

		public List<DeviceStateFieldDeclaration> Fields { get; private set; }

		public List<DeviceActionDeclaration> Actions { get; private set; }
		
		public List<DeviceEventDeclaration> Events { get; private set; }

		public DeviceInfo() {
			Fields = new List<DeviceStateFieldDeclaration>();
			Actions = new List<DeviceActionDeclaration>();
			Events = new List<DeviceEventDeclaration>();
		}

		public DeviceInfo(IReadOnlyCollection<DeviceStateFieldDeclaration> fields, 
		                  IReadOnlyCollection<DeviceActionDeclaration> actions, 
		                  IReadOnlyCollection<DeviceEventDeclaration> events) {
			Fields = fields.ToList();
			Actions = actions.ToList();
			Events = events.ToList();
		}
	}
}
