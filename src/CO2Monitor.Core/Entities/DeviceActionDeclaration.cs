namespace CO2Monitor.Core.Entities {
	public class DeviceActionDeclaration {
		public DeviceActionDeclaration(string path, VariantDeclaration argumentDeclaration) {
			Path = path;
			Argument = argumentDeclaration; 
		}

		public string Path { get; }

		public VariantDeclaration Argument { get; set; }

		public override bool Equals(object obj) {
			if (obj == null)
				return false;

			if (GetType() != obj.GetType())
				return false;

			var other = (DeviceActionDeclaration)obj;
			return (Argument.Equals(other.Argument)) && (Path == other.Path);
		}

		public override int GetHashCode() {
			return Argument.GetHashCode() + 431 * (Path == null ? 0 : Path.GetHashCode());
		}
	}
}
