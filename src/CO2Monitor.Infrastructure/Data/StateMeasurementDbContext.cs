using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Infrastructure.Data {
	public class StateMeasurementDbContext : DbContext {
#if DEBUG
		private readonly ILoggerFactory _loggerFactory;

		public StateMeasurementDbContext(ILoggerFactory loggerFactory) {
			_loggerFactory = loggerFactory;
		}
#endif

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
#if DEBUG
				.UseLoggerFactory(_loggerFactory) //tie-up DbContext with LoggerFactory object
				.EnableSensitiveDataLogging()
#endif
				.UseSqlite("Data Source=EfStateMeasurementsDb.sqlite;");
		}
	}
}

