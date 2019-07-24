using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Helpers;

namespace CO2Monitor.Core.Entities {
	public class Variant {
		private readonly string _string;
		private readonly double _float;
		private readonly TimeSpan _time; 

		public Variant() {
			Declaration = new VariantDeclaration(VariantType.Void);
		}

		public Variant(VariantDeclaration declaration, string val) {
			Declaration = declaration;
			try {
				switch (Declaration.Type) {
					case VariantType.Enum:
						val = val.ToLower();
						if (!Declaration.EnumValues.Contains(val))
							throw new CO2MonitorArgumentException(nameof(val), $"Enum [{Declaration}] does not contain value [{val}]");
						_string = val;
						break;
					case VariantType.Float:
						_float = double.Parse(val);
						break;
					case VariantType.String:
						_string = val;
						break;
					case VariantType.Time:
						_time = TimeSpan.Parse(val);
						break;
					case VariantType.Void:
						break;
					default:
						throw new NotImplementedException();
				}
			} catch (Exception ex) {
				throw new CO2MonitorArgumentException(nameof(val), $"Can not init Value with type [{Declaration}] and string value [{val}]", ex);
			}
		}

		public Variant(double value) {
			Declaration = new VariantDeclaration(VariantType.Float);
			_float = value;
		}

		public Variant(TimeSpan time) {
			Declaration = new VariantDeclaration(VariantType.Time);
			_time = time;
		}

		public Variant(string value) {
			Declaration = new VariantDeclaration(VariantType.String);
			_string = value;
		}

		public Variant(string value, IReadOnlyList<string> enumValues) {
			value = value.ToLower();
			Declaration = new VariantDeclaration(VariantType.Enum, enumValues);

			if (!enumValues.Contains(value))
				throw new CO2MonitorArgumentException($"value \"{value}\" is not in enumValues [{enumValues.Aggregate((acc, x) => acc + $"\"{x}\" ")}]");
			
			_string = value;
		}

		public VariantDeclaration Declaration { get; }

		public string Enum {
			get {
				if (Declaration.Type != VariantType.Enum)
					throw new InvalidOperationException($"Value type is {Declaration.Type} not Enum");

				return _string;
			}
		}

		public string String {
			get {
				switch (Declaration.Type) {
					case VariantType.Float:
						return _float.ToString(CultureInfo.InvariantCulture);
					case VariantType.Time:
						return _time.ToString();
					default:
						return _string ?? "";
				}
			}
		}

		public double Float {
			get {
				if (Declaration.Type != VariantType.Float)
					throw new InvalidOperationException($"Value type is {Declaration.Type} not Float");

				return _float;
			}
		}

		public TimeSpan Time {
			get {
				if (Declaration.Type != VariantType.Time)
					throw new InvalidOperationException($"Value type is {Declaration.Type} not Time");
				return _time;
			}
		}

		public bool Equals(Variant other) {
			if (other == null)
				return false;

			if (Declaration != other.Declaration)
				return false;

			switch (Declaration.Type) {
				case VariantType.Enum:
				case VariantType.String:
					return _string == other._string;
				case VariantType.Float:
					return _float == other._float;
				case VariantType.Time:
					return _time == other._time;
				case VariantType.Void:
					return true;
				default:
					throw new NotImplementedException();
			}
		}

		public override bool Equals(object obj) => Equals(obj as Variant);

		public override int GetHashCode() {
			int hash = Declaration.GetHashCode();
			switch (Declaration.Type) {
				case VariantType.Enum:
				case VariantType.String:
					return hash + 431 * _string.GetHashCode();
				case VariantType.Float:
					return hash + 431 * _float.GetHashCode();
				case VariantType.Time:
					return hash + 431 * _time.GetHashCode();
				case VariantType.Void:
					return hash;
				default:
					throw new NotImplementedException();
			}
		}

		public override string ToString() {
			switch (Declaration.Type) {
				case VariantType.Enum:
				case VariantType.String:
					return _string;
				case VariantType.Time:
					return _time.ToString();
				case VariantType.Float:
					return _float.ToString(CultureInfo.InvariantCulture);
				case VariantType.Void:
					return string.Empty;
				default:
					throw new NotImplementedException();
			}
		}

		public static bool operator ==(Variant a, Variant b) {
			if (a is null)
				return b is null;

			return a.Equals(b);
		}

		public static bool operator !=(Variant a, Variant b) {
			if (a is null)
				return !(b is null);

			return !a.Equals(b);
		}

		public static bool operator <(Variant a, Variant b) {
			if (a is null || b is null || a.Declaration != b.Declaration)
				return false;

			switch (a.Declaration.Type) {
				case VariantType.Enum:
					return a.Declaration.EnumValues.IndexOf(a._string) < b.Declaration.EnumValues.IndexOf(b._string);
				case VariantType.String:
					return string.CompareOrdinal(a._string, b._string) < 0;
				case VariantType.Time:
					return a._time < b._time;
				case VariantType.Float:
					return a._float < b._float;
				case VariantType.Void:
					return false;
				default:
					throw new NotImplementedException();
			}
		}

		public static bool operator >(Variant a, Variant b) {
			if (a is null || b is null || a.Declaration != b.Declaration)
				return false;

			switch (a.Declaration.Type) {
				case VariantType.Enum:
					return a.Declaration.EnumValues.IndexOf(a._string) > b.Declaration.EnumValues.IndexOf(b._string);
				case VariantType.String:
					return string.CompareOrdinal(a._string, b._string) > 0;
				case VariantType.Time:
					return a._time > b._time;
				case VariantType.Float:
					return a._float > b._float;
				case VariantType.Void:
					return false;
				default:
					throw new NotImplementedException();
			}
		}

		public static bool operator <=(Variant a, Variant b) {
			if (a is null || b is null || a.Declaration != b.Declaration)
				return false;

			switch (a.Declaration.Type) {
				case VariantType.Enum:
					return a.Declaration.EnumValues.IndexOf(a._string) <= b.Declaration.EnumValues.IndexOf(b._string);
				case VariantType.String:
					return string.CompareOrdinal(a._string, b._string) <= 0;
				case VariantType.Time:
					return a._time <= b._time;
				case VariantType.Float:
					return a._float <= b._float;
				case VariantType.Void:
					return true;
				default:
					throw new NotImplementedException();
			}
		}

		public static bool operator >=(Variant a, Variant b) {
			if (a is null || b is null || a.Declaration != b.Declaration)
				return false;

			switch (a.Declaration.Type) {
				case VariantType.Enum:
					return a.Declaration.EnumValues.IndexOf(a._string) >= b.Declaration.EnumValues.IndexOf(b._string);
				case VariantType.String:
					return string.CompareOrdinal(a._string, b._string) >= 0;
				case VariantType.Time:
					return a._time >= b._time;
				case VariantType.Float:
					return a._float >= b._float;
				case VariantType.Void:
					return true;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
