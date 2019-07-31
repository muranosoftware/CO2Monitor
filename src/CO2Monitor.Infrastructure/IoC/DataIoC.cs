using System;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Infrastructure.Data;

namespace CO2Monitor.Infrastructure.IoC {
	public static class DataIoC {
		public static IServiceCollection AddDataServices(this IServiceCollection services) {
			services.AddSingleton<IDeviceRepository, FileDeviceRepository>();
			services.AddSingleton<IDeviceStateRepository, SqLiteDeviceStateRepository>();
			services.AddSingleton<IActionRuleRepository, FileActionRuleRepository>();
			return services;
		}

		public static void ConfigureDataServices(this IServiceProvider serviceProvider) {
			serviceProvider.GetService<IDeviceStateRepository>().EnsureCreated();
		}
	}
}

