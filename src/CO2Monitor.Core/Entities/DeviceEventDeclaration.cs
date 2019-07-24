using Newtonsoft.Json;

namespace CO2Monitor.Core.Entities {
	public sealed class DeviceEventDeclaration {
		public DeviceEventDeclaration(string name, VariantDeclaration dataDeclaration) {
			Name = name;
			DataType = dataDeclaration;
		}

		[JsonConstructor]
		private DeviceEventDeclaration() {
			DataType = VariantDeclaration.Void;
		}

		public string Name { get; set; }
		public VariantDeclaration DataType { get; set; }

		public bool Equals(DeviceEventDeclaration other) {
			if (other is null)
				return false;

			return DataType.Equals(other.DataType) && Name == other.Name;
		}

		public override bool Equals(object obj) {
			return Equals(obj as DeviceEventDeclaration);
		}

		public override int GetHashCode() {
			return DataType.GetHashCode() + 431 * (Name is null ? 0 : Name.GetHashCode());
		}

		public override string ToString() {
			return $"{Name}({DataType})";
		}

		public static bool operator ==(DeviceEventDeclaration a, DeviceEventDeclaration b) {
			if (a is null)
				return b is null;

			return a.Equals(b);
		}

		public static bool operator !=(DeviceEventDeclaration a, DeviceEventDeclaration b) {
			if (a is null)
				return !(b is null);

			return !a.Equals(b);
		}
	}
}
