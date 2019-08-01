using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class RuleConditionViewModelVaidator : AbstractValidator<RuleConditionViewModel> {
		public RuleConditionViewModelVaidator() {
			RuleFor(x => x.ConditionArgument).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEmpty();
			RuleFor(x => x.Field).SetValidator(new FieldViewModelValidator());
		}
	}

	public class RuleViewModelValidator : AbstractValidator<RuleViewModel> {
		public RuleViewModelValidator() {
			RuleFor(x => x.Name).Matches(@"^\p{L}+(\p{L}|\p{Nd})*$")
			                    .WithMessage("Rule name must starts with letter and contains only digits and letters")
			                    .MaximumLength(50).WithMessage("Rule name length must be less than 50");
			RuleFor(x => x.Event).SetValidator(new EventViewModelValidator());
			RuleFor(x => x.Action).SetValidator(new ActionViewModelValidator());
		}
	}
}
