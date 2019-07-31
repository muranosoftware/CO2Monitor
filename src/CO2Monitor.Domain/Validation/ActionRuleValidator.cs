using System.Linq;
using FluentValidation;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Domain.Validation {
	public class ActionRuleValidator : AbstractValidator<ActionRule> {
		public ActionRuleValidator() {
			RuleFor(x => x.Action).SetValidator(new DeviceActionDeclarationValidator());
			RuleFor(x => x.Name).Length(1, 20)
			                    .WithMessage("Rule name length must be between 1 and 20")
			                    .Must(r => r.All(c => char.IsLetter(c) || char.IsDigit(c)))
			                    .WithMessage("Rule name must contains only letters and digits");
			
			RuleFor(x => x.Conditions).NotNull();
			RuleForEach(x => x.Conditions).SetValidator(new ActionConditionValidator());
		}
	}
}