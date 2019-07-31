using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Notifications {
	public class SlackEventNotifier : IEventNotifier {
		private readonly string _responseUrl;

		private readonly ILogger<SlackEventNotifier> _logger;

		public SlackEventNotifier(IConfiguration configuration, ILogger<SlackEventNotifier> logger) {
			_responseUrl = configuration.GetSection("Slack").GetValue<string>("NotifierWebHookUrl");
			_logger = logger;
		}

		public async Task Notify(string message) {
			using (var client = new HttpClient()) {
				try {
					string json = JsonConvert.SerializeObject(new {
						text = message
					});
					await client.PostAsync(_responseUrl, new StringContent(json));
				} catch (OperationCanceledException ex) {
					_logger.LogError(ex, "Can not send notification to slack");
				} catch (Exception ex) {
					_logger.LogError(ex, "Can not send notification to slack");
				}
			}
		}
	}
}
