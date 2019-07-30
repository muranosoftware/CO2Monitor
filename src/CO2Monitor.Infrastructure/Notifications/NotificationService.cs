using System.Linq;
using System.Collections.Generic;
using MoreLinq;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Notifications {
	class NotificationService : IEventNotificationService {
		private readonly ILogger<NotificationService> _logger;
		private readonly IReadOnlyCollection<IEventNotifier> _notifiers;

		public NotificationService(IEnumerable<IEventNotifier> notifiers, ILogger<NotificationService> logger) {
			_logger = logger;
			_notifiers = notifiers.ToArray();
		}

		public void Notify(string message) {
			_logger.LogInformation("Sending notifacation: " + message);
			_notifiers.ForEach( n => n.Notify(message));
		}
	}
}
