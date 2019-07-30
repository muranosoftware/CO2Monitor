using Newtonsoft.Json;

namespace CO2Monitor.Core.Entities {
	public sealed class DeviceStateFieldDeclaration {
		[JsonConstructor]
		public DeviceStateFieldDeclaration(string name, VariantDeclaration type) {
			Name = name;
			Type = type;
		}

		public string Name { get; private set; }
		public VariantDeclaration Type { get; private set; }

		public bool Equals(DeviceStateFieldDeclaration other) => other is null ? false : Type.Equals(other.Type) && Name == other.Name;

		public override bool Equals(object obj) => Equals(obj as DeviceStateFieldDeclaration);

		public override int GetHashCode() => Type.GetHashCode() + (431 * (Name is null ? 0 : Name.GetHashCode()));

		public override string ToString() => $"{Name} {Type}";
		
		public static bool operator ==(DeviceStateFieldDeclaration a, DeviceStateFieldDeclaration b) {
			return a is null ? b is null : a.Equals(b);
		}

		public static bool operator !=(DeviceStateFieldDeclaration a, DeviceStateFieldDeclaration b) {
			return a is null ? !(b is null) : !a.Equals(b);
		}
	}
}
