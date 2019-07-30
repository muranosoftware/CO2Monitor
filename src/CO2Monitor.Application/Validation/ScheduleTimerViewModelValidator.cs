using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class ScheduleTimerViewModelValidator : AbstractValidator<ScheduleTimerViewModel> {
		public ScheduleTimerViewModelValidator() {
			RuleFor(x => x.Name).Matches(@"^\p{L}+(\p{L}|\p{Nd})*$")
			                    .WithMessage("Timer name must starts with letter and contains only digits and letters")
			                    .MaximumLength(50).WithMessage("Timer name length must be less than 50");

			RuleFor(x => x.AlarmTime).Cascade(CascadeMode.StopOnFirstFailure).NotNull()
			                         .Must(x => x != null &&  0 <= x.Value.TotalHours && x.Value.TotalHours < 24)
			                         .WithMessage("Alarm time must be beetween 00:00 and 23:59:59");
		}
	}
}
