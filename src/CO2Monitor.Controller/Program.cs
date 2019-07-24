using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CO2Monitor.Controller {
	public class Program {
		public static void Main(string[] args) {
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
			       .ConfigureAppConfiguration((hostingContext, config) => {
				       config.AddIniFile("secrets.ini", optional: true, reloadOnChange: true);
			       })
			       .UseUrls("https://*:5001;http://*:5000")
			       .UseStartup<Startup>();
	}
}
