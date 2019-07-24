﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using CO2Monitor.Infrastructure.Services;
using CO2Monitor.Infrastructure.Logging;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Infrastructure.Notifications;
using CO2Monitor.Infrastructure.Validation;

namespace CO2Monitor.Controller {
	public class Startup {
		private readonly ILogger _logger;
		
		public Startup(IConfiguration configuration, ILogger<Startup> logger) {
			_logger = logger;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<DeviceInfoValidator>());

			services.AddEntityFrameworkSqlite();

			services.AddDbLoggerService<SqLiteLogRecordsRepository>();

			services.AddDeviceServices();

			services.AddNotificationServices();

			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new Info { Title = "CO2Monitor API", Version = "v1" });
				//var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				//var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				//c.IncludeXmlComments(xmlPath); TODO: make documentation
			});

			services.AddSignalR();
		}

		public void Configure(IApplicationBuilder app, 
							  IHostingEnvironment env, 
							  ILoggerFactory loggerFactory, 
							  IServiceProvider serviceProvider) {
			serviceProvider.GetService<LogRecordsRepositoryAccessor>().LogRepository.EnsureCreated();

			serviceProvider.GetService<IDeviceStateRepository>().EnsureCreated();

			serviceProvider.GetService<IDeviceTextCommandService>(); // force create singleton

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
				OnPrepareResponse = ctx => {
					ctx.Context.Response.Headers.Append(new KeyValuePair<string, StringValues>("Cache-Control", $"public, max-age={cachePeriod}"));
				}
			});

			app.UseSwagger();

			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseSignalR(routes => {
				routes.MapHub<EventHub>("/events");
			});

			app.UseMvc();
		}
	}
}
