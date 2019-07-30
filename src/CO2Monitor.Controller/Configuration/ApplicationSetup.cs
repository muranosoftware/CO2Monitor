using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Application.IoC;

namespace CO2Monitor.Controller.Configuration {
	public static class ApplicationSetup {
		public static IServiceCollection AddApplicationSetup(this IServiceCollection services) {
			services.AddApplicationServices();
			return services;
		}
	}
}
