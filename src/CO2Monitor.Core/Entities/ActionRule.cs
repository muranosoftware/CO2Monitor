namespace CO2Monitor.Core.Entities {
	public class ActionRule {
		public int Id { get; set; }
		public string Name { get; set; }

		public int SourceDeviceId { get; set; }

		public DeviceEventDeclaration Event { get; set; }

		public int TargetDeviceId { get; set; }

		public DeviceActionDeclaration Action { get; set; }

		public string ActionArgument { get; set; }
	}
}
