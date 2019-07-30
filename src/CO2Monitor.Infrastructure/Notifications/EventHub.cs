using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CO2Monitor.Infrastructure.Notifications {
	public class EventHub : Hub //, IEventNotifier
	{
		public async Task Notify(string message) => 
			await Clients.All.SendAsync("ServerEvent", message);
	}
}
