using Newtonsoft.Json;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Application.ViewModels {
	public class EventViewModel {
		[JsonConstructor]
		public EventViewModel(string name, VariantDeclaration dataType, bool isExtension = false) {
			Name = name;
			DataType = dataType;
			IsExtension = isExtension;
		}

		public string Name { get; private set; }
		public VariantDeclaration DataType { get; private set; }

		public bool IsExtension { get; private set; }

		private EventViewModel() { }
	}
}
