using FluentValidation;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Validation {
	public class DeviceInfoValidator : AbstractValidator<DeviceInfo> {
		public DeviceInfoValidator() {
			RuleFor(x => x.Fields).NotNull();
			RuleForEach(x => x.Fields).NotNull().SetValidator(new DeviceStateFieldDeclarationValidator());

			RuleFor(x => x.Actions).NotNull();
			RuleForEach(x => x.Actions).NotNull().SetValidator(new DeviceActionDeclarationValidator());

			RuleFor(x => x.Events).NotNull();
			RuleForEach(x => x.Events).NotNull().SetValidator(new DeviceEventDeclarationValidator());
		}
	}
}