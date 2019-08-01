using System;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Application.Interfaces;
using CO2Monitor.Application.Services;
using CO2Monitor.Application.ViewModelMappings;

namespace CO2Monitor.Application.IoC {
	public static class ApplicationIoC {
		public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
			services.AddSingleton<IDeviceAppService, DeviceAppService>();

			services.AddTransient<IDeviceViewModelMappingBase, ScheduleTimerViewModelMapping>();
			services.AddTransient<IDeviceViewModelMappingBase, DeviceViewModelMapping>();
			services.AddTransient<IDeviceViewModelMappingBase, RemoteDeviceViewModelMapping>();

			services.AddSingleton<IRuleAppSevice, RuleAppService>();
			services.AddSingleton<IDeviceTextCommandService, DeviceTextCommandService>();

			return services;
		}

		public static IServiceProvider ConfigureApplicationServices(this IServiceProvider serviceProvider) {
			serviceProvider.GetService<IDeviceTextCommandService>();
			return serviceProvider;
		} 
	}
}
