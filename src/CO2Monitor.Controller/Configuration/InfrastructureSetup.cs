using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Infrastructure.IoC;

namespace CO2Monitor.Controller.Configuration {
	public static class InfrastructureSetup {
		public static IServiceCollection AddInfrastructureSetup(this IServiceCollection services) =>
			services.AddInfrastructureServices();
	}
}
