using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Hosting;
using Serilog.Extensions.Logging.File;
using Swashbuckle.AspNetCore.Swagger;


using CO2Monitor.Core.Interfaces;
using CO2Monitor.Infrastructure.RemoteServices;
using CO2Monitor.Infrastructure.Services;
using CO2Monitor.Infrastructure.Data;
using CO2Monitor.Controller.Helpers;

namespace CO2Monitor.Controller
{
    public class Startup
    {
        private readonly ILogger _logger;
        
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IMeasurementRepository, SQLiteMeasurmentRepository>();

            services.AddTransient<IRemoteCO2Driver, RemoteCO2Driver>();

            services.AddTransient<IRemoteCO2FanController, FakeRemoteCO2FanController>();

            services.AddSingleton<ICO2ControllerService, CO2ControllerService>();

            services.AddHostedService<BackgroundServiceStarter<ICO2ControllerService>>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CO2Monitor API", Version = "v1" });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath); TODO: make documentation
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/C02Monitor.Controller.log");

            SQLiteMeasurmentRepository.Configure(Configuration);

            if (env.IsDevelopment())
            {
                _logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            var cachePeriod = env.IsDevelopment() ? "600" : "604800";
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(new KeyValuePair<string, StringValues>("Cache-Control", $"public, max-age={cachePeriod}"));
                }
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
