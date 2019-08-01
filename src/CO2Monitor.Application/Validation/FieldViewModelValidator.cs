using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class FieldViewModelValidator : AbstractValidator<FieldViewModel> {
		public FieldViewModelValidator() {
			RuleFor(x => x.Name).Matches("^[a-zA-Z]+[a-zA-Z0-9]*$")
			                    .WithMessage("Field name must starts with latin letter and contains only digits and latin letters")
			                    .MaximumLength(50).WithMessage("Field name length must be less than 50");

			RuleFor(x => x.Type).NotNull();
		}
	}
}
