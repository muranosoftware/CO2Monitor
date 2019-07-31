﻿using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Infrastructure.Services;
using CO2Monitor.Infrastructure.Interfaces;
using CO2Monitor.Domain.Helpers;

namespace CO2Monitor.Infrastructure.IoC {
	public static class InfrastructureIoC {
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services) {
#if !DISABLE_BOT
			services.AddSingleton<ITextCommandProvider, SlackProxyHubTextCommandProvider>();

			services.AddHostedService<BackgroundServiceStarter<ITextCommandProvider>>();

			services.AddSingleton<IDeviceTextCommandService, DeviceTextCommandService>();
#endif // !DISABLE_BOT
		
			services.AddTransient<IWorkDayCalendarService, IsDayOffDotRuCalendarService>();

			services.AddTransient<IPlotService, OxyPlotService>();

			return services;
		}
	}
}
