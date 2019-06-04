using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Shared;

namespace CO2Monitor.Infrastructure.RemoteServices
{
    public class RemoteCO2Driver : IRemoteCO2Driver
    {
        private class TempCO2Measurement
        {
            public int CO2 { get; set; }

            public float Temperature { get; set; }

            public string Time { get; set; }
        }

        private readonly ILogger _logger;

        public RemoteCO2Driver(ILogger<RemoteCO2Driver> logger)
        {
            _logger = logger;
        }

        public async Task<CO2Measurement> GetMeasurement(string address)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(address);
                    response.EnsureSuccessStatusCode();
                    
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var measurement = JsonConvert.DeserializeObject<TempCO2Measurement>(responseBody);

                    var result = new CO2Measurement()
                    {
                        CO2 = measurement.CO2,
                        Temperature = measurement.Temperature,
                        Time = DateTime.ParseExact(measurement.Time, "M/d/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture)
                    };
                    return result;
                }
                catch (TimeoutException e)
                {
                    var msg = $"Can not get data from [{address}]. Timeout expired!";
                    _logger.LogError(e, msg);

                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
                catch (HttpRequestException e)
                {
                    string msg = $"Can not get data from [{address}]: {e.Message}";
                    _logger.LogError(e, msg);

                    throw new CO2MonitorRemoteServiceException(msg, e);
                }
            }
        }
    }
}
