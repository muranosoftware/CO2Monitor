using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.IoC;

namespace CO2Monitor.Controller.Configuration {
	public static class DomainSetup {
		public static IServiceCollection AddDomainSetup(this IServiceCollection services) =>
			services.AddDomainServices();
	}
}
