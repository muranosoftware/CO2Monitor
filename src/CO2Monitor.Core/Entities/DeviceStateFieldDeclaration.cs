namespace CO2Monitor.Core.Entities {
	public sealed class DeviceStateFieldDeclaration {
		public DeviceStateFieldDeclaration(string name, VariantDeclaration fieldType) {
			Name = name;
			Type = fieldType;
		}

		public string Name { get; set; }
		public VariantDeclaration Type { get; set; }

		public bool Equals(DeviceStateFieldDeclaration other) {
			if (other is null)
				return false;

			return Type.Equals(other.Type) && Name == other.Name;
		}

		public override bool Equals(object obj) => Equals(obj as DeviceStateFieldDeclaration);

		public override int GetHashCode() {
			return Type.GetHashCode() + 431 * (Name is null ? 0 : Name.GetHashCode());
		}

		public override string ToString() => $"{Name} {Type}";
		
		public static bool operator ==(DeviceStateFieldDeclaration a, DeviceStateFieldDeclaration b) {
			if (a is null)
				return b is null;
			
			return a.Equals(b);
		}

		public static bool operator !=(DeviceStateFieldDeclaration a, DeviceStateFieldDeclaration b) {
			if (a is null)
				return !(b is null);

			return !a.Equals(b);
		}
	}
}
