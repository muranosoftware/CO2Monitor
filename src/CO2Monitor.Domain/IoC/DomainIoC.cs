using CO2Monitor.Domain.Interfaces.Devices;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Domain.Devices;
using CO2Monitor.Domain.Services;
using CO2Monitor.Domain.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CO2Monitor.Domain.IoC {
	public static class DomainIoC {
		private static void AddDevice<T>(this IServiceCollection serviceCollection) where T : class, IDevice {
			serviceCollection.AddTransient<T, T>();
			serviceCollection.AddTransient<IDeviceBuilder, DeviceBuilder<T>>();
		}

		public static IServiceCollection AddDomainServices(this IServiceCollection services) {
			services.AddDevices();

			services.AddSingleton<IDeviceManagerService, DeviceManagerService>();

			services.AddHostedService<BackgroundServiceStarter<IDeviceManagerService>>();

			return services;
		}

		private static void AddDevices(this IServiceCollection services) {
			services.AddDevice<CalendarDevice>();

			services.AddDevice<ScheduleTimer>();

			services.AddDevice<RemoteDevice>();

			services.AddTransient<IDeviceFactory, DeviceFactory>();

			services.AddTransient<IDeviceExtensionBuilder,
				DeviceExtensionBuilder<FloatToGyrColorDeviceExtension>>(serviceProvider =>
					new DeviceExtensionBuilder<FloatToGyrColorDeviceExtension>((parameter, device) =>
						new FloatToGyrColorDeviceExtension(parameter, device)));

			services.AddTransient<IDeviceExtensionFactory, DeviceExtensionFactory>();
		}
	}
}
