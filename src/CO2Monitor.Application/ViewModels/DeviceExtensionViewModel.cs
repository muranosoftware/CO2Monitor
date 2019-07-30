namespace CO2Monitor.Application.ViewModels {
	public class DeviceExtensionViewModel {
		public DeviceExtensionViewModel(string type, string parameter) {
			Type = type;
			Parameter = parameter;
		}

		public string Type { get; private set; }
		public string Parameter { get; private set; }
	}
}
