namespace CO2Monitor.Core.Entities {
	public sealed class DeviceEventDeclaration {
		public DeviceEventDeclaration(string name, VariantDeclaration dataDeclaration) {
			Name = name;
			DataType = dataDeclaration;
		}

		public string Name { get; }
		public VariantDeclaration DataType { get; }

		public bool Equals(DeviceEventDeclaration other) {
			if (other == null)
				return false;

			return (DataType.Equals(other.DataType)) && (Name == other.Name);
		}

		public override bool Equals(object obj) {
			return Equals(obj as DeviceEventDeclaration);
		}

		public override int GetHashCode() {
			return DataType.GetHashCode() + 431 * (Name == null ? 0 : Name.GetHashCode());
		}

		public static bool operator ==(DeviceEventDeclaration a, DeviceEventDeclaration b) {
			return a.Equals(b);
		}

		public static bool operator !=(DeviceEventDeclaration a, DeviceEventDeclaration b) {
			return !a.Equals(b);
		}
	}
}
