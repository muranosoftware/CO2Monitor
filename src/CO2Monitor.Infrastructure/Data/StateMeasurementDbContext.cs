using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Infrastructure.Data {
	public class StateMeasurementDbContext : DbContext {
		private const string ConfigurationDataSourceKey = "StateMeasurementDbContext:DataSource";
		private readonly ILoggerFactory _loggerFactory;
		private readonly string _dataSource;

		public StateMeasurementDbContext(ILoggerFactory loggerFactory, IConfiguration configuration) {
			_loggerFactory = loggerFactory;
			_dataSource = configuration.GetValue<string>(ConfigurationDataSourceKey);
		}

		public DbSet<DeviceStateMeasurement> Measurements { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<DeviceStateMeasurement>().HasKey(x => x.Id);
			modelBuilder.Entity<DeviceStateMeasurement>().Property(x => x.Id).ValueGeneratedOnAdd();
			modelBuilder.Entity<DeviceStateMeasurement>().HasIndex(x => new { x.DeviceId });
			modelBuilder.Entity<DeviceStateMeasurement>().HasIndex(x => new { x.Time });
			modelBuilder.Entity<DeviceStateMeasurement>().HasIndex(x => new { x.DeviceId, x.Time });
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder
				.UseLoggerFactory(_loggerFactory) //tie-up DbContext with LoggerFactory object
#if DEBUG
				.EnableSensitiveDataLogging()
#endif
				.UseSqlite($"Data Source={_dataSource};");
		}
	}
}

