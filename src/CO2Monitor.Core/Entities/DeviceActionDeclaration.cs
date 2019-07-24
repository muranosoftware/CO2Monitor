namespace CO2Monitor.Core.Entities {
	public sealed class DeviceActionDeclaration {
		public DeviceActionDeclaration(string path, VariantDeclaration argumentDeclaration) {
			Path = path;
			Argument = argumentDeclaration;
		}

		public string Path { get; set; }

		public VariantDeclaration Argument { get; set; }

		public bool Equals(DeviceActionDeclaration other) {
			if (other is null)
				return false;

			return (Argument == other.Argument) && (Path == other.Path);
		}

		public override bool Equals(object obj) {
			return Equals(obj as DeviceActionDeclaration);
		}

		public override int GetHashCode() {
			return Argument.GetHashCode() + 431 * (Path?.GetHashCode() ?? 0);
		}

		public override string ToString() {
			return $"{Path}({Argument})";
		}

		public static bool operator ==(DeviceActionDeclaration a, DeviceActionDeclaration b) {
			if (a is null)
				return b is null;

			return a.Equals(b);
		}

		public static bool operator !=(DeviceActionDeclaration a, DeviceActionDeclaration b) {
			if (a is null)
				return !(b is null);

			return !a.Equals(b);
		}
	}
}
