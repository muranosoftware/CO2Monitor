using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Infrastructure.Interfaces;
using CO2Monitor.Infrastructure.Notifications;

namespace CO2Monitor.Infrastructure.IoC {
	public static class NotificationsIoC {
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
