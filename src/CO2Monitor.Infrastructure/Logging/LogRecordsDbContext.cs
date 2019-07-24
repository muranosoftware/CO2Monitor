using CO2Monitor.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CO2Monitor.Infrastructure.Logging {
	public class LogRecordsDbContext : DbContext {
		public DbSet<LogRecord> Records { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<LogRecord>().HasKey(x => x.Id);
			modelBuilder.Entity<LogRecord>().Property(x => x.Id).ValueGeneratedOnAdd();
			modelBuilder.Entity<LogRecord>().HasIndex(x => new { x.Time });
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlite("Data Source=EfEventLog.sqlite;");
		}
	}
}