using System;
using System.Net.Http;
using System.Threading.Tasks;
using CO2Monitor.Domain.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Services {
	public class IsDayOffDotRuCalendarService : IWorkDayCalendarService {
		private const string Site = "https://isdayoff.ru/";
		private const string DateFormat = "yyyyMMdd";

		public async Task<bool> IsWorkDay(DateTime date) {
			string url = Site + date.ToString(DateFormat);

			using (var client = new HttpClient()) {
				client.Timeout = TimeSpan.FromSeconds(30);
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				string responseString = await response.Content.ReadAsStringAsync();
				return responseString == "0";
			}
		}
	}
}
