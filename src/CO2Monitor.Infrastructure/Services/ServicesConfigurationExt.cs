using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Infrastructure.Data;
using CO2Monitor.Infrastructure.Helpers;
using CO2Monitor.Infrastructure.Devices;
using CO2Monitor.Core.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Services {
	public static class ServicesConfigurationExt {
		public static IServiceCollection AddDeviceServices(this IServiceCollection services) {
			services.AddSingleton<ITextCommandProvider, SlackProxyHubTextCommandProvider>();

			services.AddHostedService<BackgroundServiceStarter<ITextCommandProvider>>();

			services.AddSingleton<IDeviceTextCommandService, DeviceTextCommandService>();

			services.AddSingleton<StateMeasurementDbContext>();

			services.AddSingleton<IDeviceStateRepository, SqLiteDeviceStateRepository>();

			services.AddTransient<IDeviceRepository, FileDeviceRepository>();

			services.AddTransient<IWorkDayCalendarService, IsDayOffDotRuCalendarService>();

			services.AddTransient<IActionRuleRepository, FileActionRuleRepository>();

			services.AddSingleton<IDeviceManagerService, DeviceManagerService>();

			services.AddHostedService<BackgroundServiceStarter<IDeviceManagerService>>();

			services.AddTransient<IPlotService, OxyPlotService>();

			services.AddDevices();

			return services;
		}
	}
}
