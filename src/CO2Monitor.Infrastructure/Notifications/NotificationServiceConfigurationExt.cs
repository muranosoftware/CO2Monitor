using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Notifications {
	public static class NotificationServiceConfigurationExt {
		public static IServiceCollection AddNotificationServices(this IServiceCollection services) {
			services.AddTransient<IEventNotifier, SignalREventNotifier>();
			services.AddTransient<IEventNotifier, SlackEventNotifier>();
			services.AddTransient<IEventNotificationService, NotificationService>();

			return services;
		}
	}
}
