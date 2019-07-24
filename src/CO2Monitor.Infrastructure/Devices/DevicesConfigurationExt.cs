using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public static class DevicesConfigurationExt {
		internal static void AddDevice<T>(this IServiceCollection serviceCollection) where T : class, IDevice {
			serviceCollection.AddTransient<T, T>();
			serviceCollection.AddTransient<IDeviceBuilder, DeviceBuilder<T>>();
		}

		public static IServiceCollection AddDevices(this IServiceCollection services) {
			services.AddDevice<CalendarDevice>();

			services.AddDevice<ScheduleTimer>();

			services.AddDevice<RemoteDevice>();

			services.AddTransient<IDeviceFactory, DeviceFactory>();

			services.AddTransient<IDeviceExtensionBuilder, 
				DeviceExtensionBuilder<FloatToGyrColorDeviceExtension>>(serviceProvider => 
					                                                        new DeviceExtensionBuilder<FloatToGyrColorDeviceExtension>((parameter, device) => 
						                                                                                                                   new FloatToGyrColorDeviceExtension(parameter, device)));

			services.AddTransient<IDeviceExtensionFactory, DeviceExtensionFactory>();

			return services;
		}		
	}
}
