using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Shared;

namespace CO2Monitor.Infrastructure.RemoteServices
{
    public class RemoteCO2FanController : IRemoteCO2FanController
    {
        private ILogger _logger;


        public RemoteCO2FanController(ILogger<RemoteCO2FanController> logger)
        {
            _logger = logger;
        }

        public async Task SetCommamd(string address, FanCommand command)
        {
            var url = address + "/light/" + command.ToString().ToLower();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                try
                {
                    HttpResponseMessage response = await client.PutAsync(url, new StringContent(string.Empty));
                    response.EnsureSuccessStatusCode();
                }
                catch (TimeoutException e)
                {
                    var msg = $"Can not set command for [{address}]. Timeout expired!";
                    _logger.LogError(e, msg);

                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
                catch (HttpRequestException e)
                {
                    string msg = $"Can not set command for [{address}]: {e.Message}";
                    _logger.LogError(e, msg);

                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
            }
        }

        public async Task SetLed(string address, FanLed led)
        {
            var url = address + "/state/" + led.ToString().ToLower();
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                try
                {
                    HttpResponseMessage response = await client.PutAsync(url, new StringContent(string.Empty));
                    response.EnsureSuccessStatusCode();
                }
                catch (TimeoutException e)
                {
                    var msg = $"Can not set state for [{address}]. Timeout expired!";
                    _logger.LogError(e, msg);

                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
                catch (HttpRequestException e)
                {
                    string msg = $"Can not set state for [{address}]: {e.Message}";
                    _logger.LogError(e, msg);

                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
            }
        }
    }
}
