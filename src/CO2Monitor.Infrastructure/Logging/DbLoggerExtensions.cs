using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Logging {
	public static class DbLoggerExtensions {
		private static ILoggerFactory AddDbLogger(this ILoggerFactory factory, IServiceProvider serviceProvider, Func<string, LogLevel, bool> filter = null) {
			factory.AddProvider(new DbLoggerProvider(filter, new LogRecordsRepositoryAccessor(serviceProvider.GetService<ILogRecordsRepository>())));
			return factory;
		}

		public static ILoggerFactory AddDbLogger(this ILoggerFactory factory, LogLevel minLevel, IServiceProvider serviceProvider) {
			serviceProvider.GetService<LogRecordsRepositoryAccessor>().LogRepository.EnsureCreated();
			return AddDbLogger(factory, serviceProvider, (_, logLevel) => logLevel >= minLevel);
		}

		public static IServiceCollection AddDbLoggerService<TLogRecordsRepository>(this IServiceCollection services) where TLogRecordsRepository : class, ILogRecordsRepository {
			services.AddSingleton<LogRecordsDbContext>();

			services.AddSingleton<LogRecordsRepositoryAccessor>();

			services.AddTransient<ILogRecordsRepository, TLogRecordsRepository>();

			services.AddTransient<ILogViewer, DbLogViewer>();

			return services;
		} 
	}

	public class LogRecordsRepositoryAccessor {
		public ILogRecordsRepository LogRepository { get; }

		public LogRecordsRepositoryAccessor(ILogRecordsRepository logRecordsRepository) {
			LogRepository = logRecordsRepository;
		}
	}
}
