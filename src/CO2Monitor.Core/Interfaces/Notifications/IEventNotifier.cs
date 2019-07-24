using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces.Notifications {
	public interface IEventNotifier {
		Task Notify(string message);
	}
}
