using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class DeviceExtensionViewModelValidator : AbstractValidator<DeviceExtensionViewModel> {
		public DeviceExtensionViewModelValidator() {
			RuleFor(x => x.Type).NotEmpty();
		}
	}
}
