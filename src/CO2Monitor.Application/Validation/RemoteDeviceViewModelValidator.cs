using System;
using FluentValidation;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Validation {
	public class RemoteDeviceViewModelValidator : AbstractValidator<RemoteDeviceViewModel> {
		public RemoteDeviceViewModelValidator() {
			RuleFor(x => x.Name).Matches(@"^\p{L}+(\p{L}|\p{Nd})*$")
								.WithMessage("Device name must starts with letter and contains only digits and letters")
								.MaximumLength(50).WithMessage("Device name length must be less than 50");

			RuleFor(x => x.Address).Cascade(CascadeMode.StopOnFirstFailure)
			                       .NotNull().Must(x => Uri.IsWellFormedUriString(x.OriginalString, UriKind.Absolute)).WithMessage("Address must be http(s) link")
			                       .Must(x => x.Scheme == "http" || x.Scheme == "https").WithMessage("Address must be http(s) link");

			RuleFor(x => x.Info).SetValidator(new DeviceInfoViewModelValidator());
		}
	}
}
