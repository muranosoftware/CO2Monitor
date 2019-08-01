using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Logging {
	public class LogRecordsDbContext : DbContext {
		private readonly string _connectionString;

		public LogRecordsDbContext(IConfiguration configuration) {
			string dataSource = configuration.GetValue<string>("DbLogger:LogRecordsDbContext:DataSource");
			_connectionString = $"Data Source={dataSource};";
		}

		public DbSet<LogRecord> Records { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<LogRecord>().HasKey(x => x.Id);
			modelBuilder.Entity<LogRecord>().Property(x => x.Id).ValueGeneratedOnAdd();
			modelBuilder.Entity<LogRecord>().HasIndex(x => new { x.Time });
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
			optionsBuilder.UseSqlite(_connectionString);
	}
}