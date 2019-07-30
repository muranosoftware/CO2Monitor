using CO2Monitor.Core.Entities;

namespace CO2Monitor.Application.ViewModels {
	public class FieldViewModel {
		public FieldViewModel(string name, VariantDeclaration type, bool isExtension = false) {
			Name = name;
			Type = type;
			IsExtension = isExtension;
		}

		public string Name { get; private set; }

		public VariantDeclaration Type { get; private set; }

		public bool IsExtension { get; private set; }
	}
}
