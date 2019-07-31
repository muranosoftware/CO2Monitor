using Newtonsoft.Json;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Domain.Entities {
	public sealed class DeviceEventDeclaration {
		[JsonConstructor]
		public DeviceEventDeclaration(string name, VariantDeclaration dataType) {
			Name = name;
			DataType = dataType;
		}
		
		public string Name { get; private set; }
		public VariantDeclaration DataType { get; private set; }

		public bool Equals(DeviceEventDeclaration other) => other is null ? false : DataType.Equals(other.DataType) && Name == other.Name;

		public override bool Equals(object obj) => Equals(obj as DeviceEventDeclaration);

		public override int GetHashCode() => DataType.GetHashCode() + (431 * (Name is null ? 0 : Name.GetHashCode()));

		public override string ToString() => $"{Name}({DataType})";

		public static bool operator ==(DeviceEventDeclaration a, DeviceEventDeclaration b) {
			return a is null ? b is null : a.Equals(b);
		}

		public static bool operator !=(DeviceEventDeclaration a, DeviceEventDeclaration b) {
			return a is null ? !(b is null) : !a.Equals(b);
		}
	}
}
