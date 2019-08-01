using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Services {
	public class SlackProxyHubTextCommandProvider : ITextCommandProvider, IDisposable {
		public class Message {
			public string Text { get; set; }
			public string UserName { get; set; }
			public string UserId { get; set; }
			public string ConversationId { get; set; }
			public bool? IsGroup { get; set; }
		}

		private readonly string _apiToken;

		private readonly HubConnection _connection;

		private static Timer _checkConnectionTimer;

		private readonly ILogger<SlackProxyHubTextCommandProvider> _logger;

		public event TextCommandHandler NewCommand;

		public SlackProxyHubTextCommandProvider(IConfiguration configuration, ILogger<SlackProxyHubTextCommandProvider> logger) {
			_logger = logger;
			string proxyHubUrl = configuration.GetSection("Slack").GetValue<string>("ProxyBotHubUrl");
			_apiToken = configuration.GetSection("Slack").GetValue<string>("BotToken");
			_connection = new HubConnectionBuilder().WithUrl(proxyHubUrl).Build();

			_connection.Closed += async error => {
				Console.WriteLine(error.ToString());
				await Task.Delay(new Random().Next(0, 5) * 1000);
				await _connection.StartAsync();
			};

			_connection.On<string, string>("ReceiveMessage", OnMessageHandler);

			_checkConnectionTimer = new Timer(CheckConnection, null, TimeSpan.FromMinutes(0.5), TimeSpan.FromMinutes(3));
		}

		private void OnMessageHandler(string user, string message) {
			if (user == "co2Monitor" && message == "ping") {
				return;
			}

			try {
				var msg = JsonConvert.DeserializeObject<Message>(message);
				NewCommand?.Invoke(msg.UserId.Split(':').FirstOrDefault(), msg.UserName, msg.Text, msg.ConversationId.Split(':').LastOrDefault(), msg.IsGroup ?? false);
			} catch (JsonReaderException ex) {
				_logger.LogError(ex, "Invalid message from Slack Proxy");
			}
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			_logger.LogTrace("HubTextCommandProvide starting");
			return _connection.StartAsync(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			_logger.LogTrace("HubTextCommandProvide stopped");
			return _connection.StopAsync(cancellationToken);
		}

		public async Task SendTextMessage(string channelId, string message, object[] attachments = null) {
			_logger.LogTrace($"Sending to channel [{channelId}] message [{message}]");

			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				try {
					var data = new List<KeyValuePair<string, string>> {
						new KeyValuePair<string, string>("text", message),
					};

					if (attachments != null) {
						data.Add(new KeyValuePair<string, string>("attachments", JsonConvert.SerializeObject(attachments)));
					}

					var form = new FormUrlEncodedContent(data);

					HttpResponseMessage response = await client.PostAsync($"https://slack.com/api/chat.postMessage?token={_apiToken}&channel={channelId}", form);
					await response.Content.ReadAsStringAsync();
				} catch (OperationCanceledException ex) {
					_logger.LogError(ex, "Can not send text message");
				} catch (HttpRequestException ex) {
					_logger.LogError(ex, "Can not send text message");
				}
			}
		}

		public async Task SendFileMessage(string channelId, Stream stream, string title) {
			using (var content = new MultipartFormDataContent()) {
				content.Headers.ContentType.MediaType = "multipart/form-data";
				content.Add(new StreamContent(stream), "file", title);

				try {
					using (var httpClient = new HttpClient()) {
						httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

						HttpResponseMessage response = await httpClient.PostAsync(new Uri($"https://slack.com/api/files.upload?token={_apiToken}&channels={channelId}"), content);
						await response.Content.ReadAsStringAsync();
					}
				} catch (OperationCanceledException ex) {
					_logger.LogError(ex, "Can not file text message");
				} catch (HttpRequestException ex) {
					_logger.LogError(ex, "Can not file text message");
				}
			}
		}

		private async void CheckConnection(object state) {
			try {
				if (_connection.State == HubConnectionState.Disconnected) {
					await _connection.StartAsync();
				} else {
					await _connection.SendCoreAsync("ReceiveMessage", new object[] { "co2Monitor", "ping" });
				}
			} catch (Exception ex) {
				_logger.LogError(ex, "Can not connect to SlackProxy hub");
			}
		}

		public void Dispose() => _checkConnectionTimer.Dispose();
	} 
}
