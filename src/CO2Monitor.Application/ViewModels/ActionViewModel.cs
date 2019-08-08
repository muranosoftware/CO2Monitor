using Newtonsoft.Json;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Application.ViewModels {
	public class ActionViewModel {
		[JsonConstructor]
		public ActionViewModel(string path, VariantDeclaration argument, bool isExtension) {
			Path = path;
			Argument = argument;
			IsExtension = isExtension;
		}

		public string Path { get; private set; }

		public VariantDeclaration Argument { get; private set; }

		public bool IsExtension { get; private set; }
				
		public override string ToString() => $"{Path}({Argument})" + (IsExtension ? " [extension]"  : string.Empty);

		private ActionViewModel() { }
	}
}
