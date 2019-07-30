using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class DeviceInfoViewModelValidator : AbstractValidator<DeviceInfoViewModel> {
		public DeviceInfoViewModelValidator() {
			RuleFor(x => x.Actions).NotNull();
			RuleForEach(x => x.Actions).SetValidator(new ActionViewModelValidator());

			RuleFor(x => x.Events).NotNull();
			RuleForEach(x => x.Events).SetValidator(new EventViewModelValidator());

			RuleFor(x => x.Fields).NotNull();
			RuleForEach(x => x.Fields).SetValidator(new FieldViewModelValidator());
		}
	}
}
