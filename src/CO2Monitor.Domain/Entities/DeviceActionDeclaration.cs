using Newtonsoft.Json;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Domain.Entities {
	public sealed class DeviceActionDeclaration {
		[JsonConstructor]
		public DeviceActionDeclaration(string path, VariantDeclaration argument) {
			Path = path;
			Argument = argument;
		}

		public string Path { get; private set; }

		public VariantDeclaration Argument { get; private set; }

		public bool Equals(DeviceActionDeclaration other) => other is null ? false : (Argument == other.Argument) && (Path == other.Path);

		public override bool Equals(object obj) => Equals(obj as DeviceActionDeclaration);

		public override int GetHashCode() => Argument.GetHashCode() + (431 * (Path?.GetHashCode() ?? 0));

		public override string ToString() => $"{Path}({Argument})";

		public static bool operator ==(DeviceActionDeclaration a, DeviceActionDeclaration b) {
			return a is null ? b is null : a.Equals(b);
		}

		public static bool operator !=(DeviceActionDeclaration a, DeviceActionDeclaration b) {
			return a is null ? !(b is null) : !a.Equals(b);
		}
	}
}
