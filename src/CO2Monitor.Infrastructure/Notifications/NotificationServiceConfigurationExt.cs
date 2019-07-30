using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Notifications {
	public static class NotificationServiceConfigurationExt {
		public static IServiceCollection AddNotificationServices(this IServiceCollection services) {
			services.AddTransient<IEventNotifier, SignalREventNotifier>();

#if !DISABLE_BOT
			services.AddTransient<IEventNotifier, SlackEventNotifier>();
#endif // !DISABLE_BOT
			services.AddTransient<IEventNotificationService, NotificationService>();

			return services;
		}
	}
}
