using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Notifications {
	public class SignalREventNotifier : IEventNotifier {
		readonly IHubContext<EventHub> _eventHubContext;

		public SignalREventNotifier(IHubContext<EventHub> eventHubContext) {
			_eventHubContext = eventHubContext;
		}

		public async Task Notify(string message) => await _eventHubContext.Clients.All.SendCoreAsync("ServerEvent", new object[] { message });
	}
}
