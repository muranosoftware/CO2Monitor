using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Devices {
	public static class ServiceCollectionDeviceBuilderExt {
		public static void AddDevice<T>(this IServiceCollection serviceCollection) where T : class, IDevice {
			serviceCollection.AddTransient<T, T>();
			serviceCollection.AddTransient<IDeviceBuilder, DeviceBuilder<T>>();
		}
	}
}
