using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CO2Monitor.Core.Entities {
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ConditionType {
		Equal,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		NotEqual,
	}

	public class ActionCondition {
		public int DeviceId { get; set; }

		public DeviceStateFieldDeclaration Field { get; set; }

		public ConditionType ConditionType { get; set; }

		public string ConditionArgument { get; set; }

		public override string ToString() => $"{Field} {ConditionType.ToString()} {ConditionArgument}";
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum RuleActionArgumentSource {
		Constant,
		EventData
	}

	public class ActionRule {
		public int Id { get; set; }
		public string Name { get; set; }

		public int SourceDeviceId { get; set; }

		public DeviceEventDeclaration Event { get; set; }

		public int TargetDeviceId { get; set; }

		public DeviceActionDeclaration Action { get; set; }

		public RuleActionArgumentSource ArgumentSource { get; set; }

		public string ActionArgument { get; set; }

		public List<ActionCondition> Conditions { get; set; }
	}
}
