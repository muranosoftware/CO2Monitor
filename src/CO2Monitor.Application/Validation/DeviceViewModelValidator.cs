using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class DeviceViewModelValidator : AbstractValidator<DeviceViewModel> {
		public DeviceViewModelValidator() {
			RuleFor(x => x.Name).Matches(@"^\p{L}+(\p{L}|\p{Nd})*$")
			                    .WithMessage("Device name must starts with letter and contains only digits and letters")
			                    .MaximumLength(50).WithMessage("Device name length must be less than 50");
		}
	}
}
