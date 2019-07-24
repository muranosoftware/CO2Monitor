using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Infrastructure.Notifications {
	public class SignalREventNotifier : IEventNotifier {
		readonly IHubContext<EventHub> _eventHubContext;

		public SignalREventNotifier(IHubContext<EventHub> eventHubContext) {
			_eventHubContext = eventHubContext;
		}

		public async Task Notify(string message) {
			await _eventHubContext.Clients.All.SendCoreAsync("ServerEvent", new object[] { message });
		}
	}
}
