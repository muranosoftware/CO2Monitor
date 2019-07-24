using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces.Notifications {
	public interface IEventNotificationService {
		Task Notify(string message);
	}
}
