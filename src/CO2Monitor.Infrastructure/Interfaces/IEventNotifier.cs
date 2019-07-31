using System.Threading.Tasks;

namespace CO2Monitor.Infrastructure.Interfaces {
	public interface IEventNotifier {
		Task Notify(string message);
	}
}
