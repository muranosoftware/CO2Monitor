using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CO2Monitor.Core.Entities {
	[JsonConverter(typeof(StringEnumConverter))]
	public enum VariantType {
		Void,
		Float,
		Enum,
		String,
		Time,
	}

	public sealed class VariantDeclaration {
		public static VariantDeclaration BooleanEnum => new VariantDeclaration(VariantType.Enum, new[] { "false", "true" });

		public static VariantDeclaration Float => new VariantDeclaration(VariantType.Float);

		public static VariantDeclaration String => new VariantDeclaration(VariantType.String);

		public static VariantDeclaration Void => new VariantDeclaration(VariantType.Void);

		public static VariantDeclaration Time => new VariantDeclaration(VariantType.Time);

		IReadOnlyList<string> _enumValues;

		[JsonConstructor]
		private VariantDeclaration() {
			Type = VariantType.Void;
		}

		public VariantDeclaration(VariantType type, IReadOnlyList<string> enumValues = null) {
			Type = type;

			if (type == VariantType.Enum) {
				if (enumValues is null || enumValues.Count == 0)
					throw new ArgumentException("enumValues must be not null and has at least one element!");
				EnumValues = enumValues.Select(x => x.ToLower()).ToList();
			} else if (enumValues != null)
				throw new ArgumentException("Can not set enumValues for non enum ValueDescription");
		}

		public VariantType Type { get; set; }

		public IReadOnlyList<string> EnumValues {
			get => _enumValues;
			set { _enumValues = value?.Select(x => x.ToLower()).ToList(); }
		}

		public bool Equals(VariantDeclaration other) {
			if (other is null)
				return false;

			if (Type != other.Type)
				return false;

			if (Type == VariantType.Enum) {
				if (EnumValues.Count != other.EnumValues.Count)
					return false;

				for (int i = 0; i < EnumValues.Count; i++)
					if (EnumValues[i] != other.EnumValues[i])
						return false;
			}
			return true;
		}

		public override bool Equals(object obj) {
			return Equals(obj as VariantDeclaration);
		}

		public override int GetHashCode() {
			int hash = (int)Type;
			if (Type == VariantType.Enum) {
				for (int i = 0; i < EnumValues.Count; i++)
					hash = 431 * hash + EnumValues[i].GetHashCode();
			}
			return hash;
		}

		public override string ToString() {
			switch (Type) {
				case VariantType.Enum:
					return VariantType.Enum.ToString().ToLower() + ": " + EnumValues.Aggregate((acc, x) => acc + ", " + x);
				default:
					return Type.ToString().ToLower();
			}
		}
		public static VariantDeclaration Parse(string valueDeclaration) {
			if (string.IsNullOrEmpty(valueDeclaration))
				throw new ArgumentException("Declaration can not be null or whitespace");

			string[] splits = valueDeclaration.Split(':');

			var type = Enum.Parse<VariantType>(splits[0].Trim(), true);
			IReadOnlyList<string> enumValues = null;
			if (type == VariantType.Enum) {
				if (splits.Length != 2)
					throw new ArgumentException("Invalid format: " + valueDeclaration);
				enumValues = splits[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower().Trim()).ToList();
			} else {
				if (splits.Length != 1)
					throw new ArgumentException("Invalid format: " + valueDeclaration);
			}

			return new VariantDeclaration(type, enumValues);
		}

		public static bool operator ==(VariantDeclaration a, VariantDeclaration b) {
			if (a is null)
				return b is null;

			return a.Equals(b);
		}

		public static bool operator !=(VariantDeclaration a, VariantDeclaration b) {
			if (a is null)
				return !(b is null);

			return !a.Equals(b);
		}
	}
}
