using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Notifications {
	class NotificationService : IEventNotificationService {
		private readonly ILogger<NotificationService> _logger;
		private readonly List<IEventNotifier> _notifiers;

		public NotificationService(IEnumerable<IEventNotifier> notifiers, ILogger<NotificationService> logger) {
			_logger = logger;
			_notifiers = notifiers.ToList();
		}

		public Task Notify(string message) {
			_logger.LogInformation("Sending notifacation: " + message);
			foreach (IEventNotifier n in _notifiers)
				n.Notify(message);
			return Task.CompletedTask;
		}
	}
}
