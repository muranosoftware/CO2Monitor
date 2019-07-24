using System.Linq;
using FluentValidation;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Validation {
	class DeviceActionDeclarationValidator : AbstractValidator<DeviceActionDeclaration> {
		public DeviceActionDeclarationValidator() {
			RuleFor(x => x.Path).Length(1, 20)
			                    .WithMessage("Path length must be between 1 and 20")
			                    .Must(r => r.All(c => char.IsLetter(c) || char.IsDigit(c)))
			                    .WithMessage("Path must contains only letters and digits");
			RuleFor(x => x.Argument).NotNull();
		}
	}
}
