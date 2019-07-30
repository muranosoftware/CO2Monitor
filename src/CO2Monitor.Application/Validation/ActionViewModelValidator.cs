using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class ActionViewModelValidator : AbstractValidator<ActionViewModel> {
		public ActionViewModelValidator() {
			RuleFor(x => x.Path).Matches("^[a-z]+[a-z,0-9]*$")
								.WithMessage("Action path must starts with latin letter and contains only digits and latin letters")
								.MaximumLength(50).WithMessage("Action path length must be less than 50");

			RuleFor(x => x.Argument).NotNull();
		}
	}
}
