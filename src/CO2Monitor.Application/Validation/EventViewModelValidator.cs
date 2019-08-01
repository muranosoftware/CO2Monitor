using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class EventViewModelValidator : AbstractValidator<EventViewModel> {
		public EventViewModelValidator() {
			RuleFor(x => x.Name).Matches("^[a-zA-Z]+[a-zA-Z0-9]*$")
			                    .WithMessage("Event name must starts with latin letter and contains only digits and latin letters")
			                    .MaximumLength(50).WithMessage("Event name length must be less than 50");
			RuleFor(x => x.DataType).NotNull();
		}
	}
}
