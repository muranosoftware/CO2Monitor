using FluentValidation;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Validation {
	public class ActionConditionValidator : AbstractValidator<ActionCondition> {
		public ActionConditionValidator() {
			RuleFor(x => x.Field).NotNull();
			RuleFor(x => x.ConditionArgument).NotNull();
		}
	}
}