using System;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using CO2Monitor.Application.AutoMapper;

namespace CO2Monitor.Controller.Configuration {
	public static class AutoMapperSetup {
		public static IServiceCollection AddAutoMapperSetup(this IServiceCollection services) {
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // scan not working

			AutoMapperConfig.RegisterMappings();
			return services;
		}
	}
}
