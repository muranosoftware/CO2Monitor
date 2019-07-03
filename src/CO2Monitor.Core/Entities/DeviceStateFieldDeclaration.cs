namespace CO2Monitor.Core.Entities {
	public sealed class DeviceStateFieldDeclaration {
		public DeviceStateFieldDeclaration(string name, VariantDeclaration fieldType) {
			Name = name;
			Type = fieldType;
		}

		public string Name { get; private set; }
		public VariantDeclaration Type { get; private set; }

		public override bool Equals(object obj) {
			if (obj == null)
				return false;

			if (GetType() != obj.GetType())
				return false;

			var other = (DeviceStateFieldDeclaration)obj;
			return (Type == other.Type) && (Name == other.Name);
		}

		public override int GetHashCode() {
			return Type.GetHashCode() + 431 * (Name == null ? 0 : Name.GetHashCode());
		}
	}
}
