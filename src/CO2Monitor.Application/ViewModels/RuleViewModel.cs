using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CO2Monitor.Application.ViewModels {
	
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ConditionType {
		Equal,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		NotEqual,
	}

	public class RuleConditionViewModel {
		public int DeviceId { get; set; }

		public FieldViewModel Field { get; set; }

		public ConditionType ConditionType { get; set; }

		public string ConditionArgument { get; set; }

		public override string ToString() => $"{Field} {ConditionType.ToString()} {ConditionArgument}";
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum RuleActionArgumentSource {
		Constant,
		EventData
	}

	public class RuleViewModel {
		public int Id { get; set; }
		public string Name { get; set; }

		public int SourceDeviceId { get; set; }

		public EventViewModel Event { get; set; }

		public int TargetDeviceId { get; set; }

		public ActionViewModel Action { get; set; }

		public RuleActionArgumentSource ArgumentSource { get; set; }

		public string ActionArgument { get; set; }

		public List<RuleConditionViewModel> Conditions { get; set; }
	}
}

