using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.Swagger;
using CO2Monitor.Infrastructure.Logging;
using CO2Monitor.Infrastructure.Notifications;
using CO2Monitor.Controller.Configuration;
using CO2Monitor.Controller.Filters;
using CO2Monitor.Infrastructure.IoC;

namespace CO2Monitor.Controller {
	public class Startup {
		private readonly ILogger _logger;
		
		public Startup(IConfiguration configuration, ILogger<Startup> logger) {
			_logger = logger;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc(opt => opt.Filters.Add(new ModelStateFilter()))
			        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
			        .AddFluentValidationSetup();

			services.AddSignalR();

			services.AddAutoMapperSetup();

			//services.AddEntityFrameworkSqlite();

			services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "CO2Monitor API", Version = "v1" }));

			services.AddDbLoggerService<SqLiteLogRecordsRepository>();

			services.AddDomainSetup();

			services.AddInfrastructureSetup();

			services.AddDataSetup();

			services.AddNotificationServices();

			services.AddApplicationSetup();
		}

		public void Configure(IApplicationBuilder app, 
		                      IHostingEnvironment env, 
		                      ILoggerFactory loggerFactory, 
		                      IServiceProvider serviceProvider) {
			serviceProvider.ConfigureDataSetup();
			serviceProvider.ConfigureApplicationSetup();


			loggerFactory.AddFile("Logs/C02Monitor.Controller.log");
			loggerFactory.AddDbLogger(LogLevel.Information, serviceProvider);

			if (env.IsDevelopment()) {
				_logger.LogInformation("In Development environment");
				app.UseDeveloperExceptionPage();
			} else {
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseDefaultFiles();
			string cachePeriod = env.IsDevelopment() ? "600" : "604800";
			app.UseStaticFiles(new StaticFileOptions {
				OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append(new KeyValuePair<string, StringValues>("Cache-Control", $"public, max-age={cachePeriod}"))
			});

			app.UseSwagger();

			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

			app.UseSignalR(routes => routes.MapHub<EventHub>("/events"));

			app.UseMvc();
		}
	}
}
