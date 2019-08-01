using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Validation;
using CO2Monitor.Application.Validation;

namespace CO2Monitor.Controller.Configuration {
	public static class FluentValidationSetup {
		public static IMvcBuilder AddFluentValidationSetup(this IMvcBuilder mvcBuilder) {
			return mvcBuilder.AddFluentValidation(fv => {
			                                      fv.RegisterValidatorsFromAssemblyContaining<DeviceViewModelValidator>();
			                                      fv.RegisterValidatorsFromAssemblyContaining<DeviceInfoValidator>();
			 });
		}
	}
}
