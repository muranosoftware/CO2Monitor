using System.Linq;
using FluentValidation;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Validation {
	public class DeviceInfoValidator : AbstractValidator<DeviceInfo> {
		public DeviceInfoValidator() {
			RuleFor(x => x.Fields).NotNull();
			RuleForEach(x => x.Fields).NotNull().SetValidator(new DeviceStateFieldDeclarationValidator());
			RuleFor(x => x.Fields).Must(y => y.All(f => y.Count(z => z.Name == f.Name) == 1)).WithMessage("Field names must be unique");

			RuleFor(x => x.Actions).NotNull();
			RuleForEach(x => x.Actions).NotNull().SetValidator(new DeviceActionDeclarationValidator());
			RuleFor(x => x.Actions).Must(y => y.All(a => y.Count(z => z.Path == a.Path) == 1)).WithMessage("Action paths must be unique");

			RuleFor(x => x.Events).NotNull();
			RuleForEach(x => x.Events).NotNull().SetValidator(new DeviceEventDeclarationValidator());
			RuleFor(x => x.Events).Must(y => y.All(e => y.Count(z => z.Name == e.Name) == 1)).WithMessage("Event names must be unique");
		}
	}
}