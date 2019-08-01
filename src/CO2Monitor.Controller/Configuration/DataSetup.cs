using System;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Infrastructure.IoC;

namespace CO2Monitor.Controller.Configuration {
	public static class DataSetup {
		public static IServiceCollection AddDataSetup(this IServiceCollection services) => services.AddDataServices();

		public static IServiceProvider ConfigureDataSetup(this IServiceProvider serviceProvider) => serviceProvider.ConfigureDataServices();
	}
}
