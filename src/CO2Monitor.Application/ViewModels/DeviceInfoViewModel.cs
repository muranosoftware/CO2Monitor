using System.Collections.Generic;

namespace CO2Monitor.Application.ViewModels {
	public class DeviceInfoViewModel {
		public DeviceInfoViewModel(IReadOnlyCollection<FieldViewModel> fields, 
		                           IReadOnlyCollection<ActionViewModel> actions,
		                           IReadOnlyCollection<EventViewModel> events) {
			Fields = fields;
			Actions = actions;
			Events = events;
		}

		public IReadOnlyCollection<FieldViewModel> Fields { get; private set; }

		public IReadOnlyCollection<ActionViewModel> Actions { get; private set; }

		public IReadOnlyCollection<EventViewModel> Events { get; private set; }
	}
}
