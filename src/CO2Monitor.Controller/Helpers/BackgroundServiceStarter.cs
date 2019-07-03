using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace CO2Monitor.Controller.Helpers {
	public class BackgroundServiceStarter<T> : IHostedService where T : IHostedService {
		readonly T _backgroundService;

		public BackgroundServiceStarter(T backgroundService) {
			_backgroundService = backgroundService;
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			return _backgroundService.StartAsync(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			return _backgroundService.StopAsync(cancellationToken);
		}
	}
}
