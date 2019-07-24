using System.Linq;
using FluentValidation;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Validation {
	public class DeviceStateFieldDeclarationValidator : AbstractValidator<DeviceStateFieldDeclaration> {
		public DeviceStateFieldDeclarationValidator() {
			RuleFor(x => x.Name).Length(1, 20)
			                    .WithMessage("Field name length must be between 1 and 20")
			                    .Must(r => r.All(c => char.IsLetter(c) || char.IsDigit(c)))
			                    .WithMessage("Field name must contains only letters and digits");
			RuleFor(x => x.Type).NotNull();
		}
	}
}