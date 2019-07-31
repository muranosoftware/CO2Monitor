using FluentValidation;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Domain.Validation {
	public class ActionConditionValidator : AbstractValidator<ActionCondition> {
		public ActionConditionValidator() {
			RuleFor(x => x.Field).NotNull();
			RuleFor(x => x.ConditionArgument).NotNull();
		}
	}
}