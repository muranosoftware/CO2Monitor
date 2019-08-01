using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Dapper;
using CO2Monitor.Infrastructure.Helpers.Dapper;
using CO2Monitor.Infrastructure.Interfaces;

namespace CO2Monitor.Infrastructure.Logging {
	public class SqLiteLogRecordsRepository : ILogRecordsRepository {
		private const string ConfigurationDataSourceKey = "DbLogger:SqLiteLogRecordsRepository:DataSource";
		private static readonly SqLiteTableMapping<LogRecord> Mapping = new SqLiteTableMapping<LogRecord>("LogRecords");

		private readonly string _dataSource;

		private readonly string _connectionString;

		public SqLiteLogRecordsRepository(IConfiguration configuration) {
			_dataSource = configuration.GetValue<string>(ConfigurationDataSourceKey);
			_connectionString = $"Data Source={_dataSource};";
		}

		public void Add(LogRecord measurement) {
			using (var conn = new SQLiteConnection(_connectionString)) {
				conn.Execute(Mapping.CreateSql, measurement);
			}
		}

		public IEnumerable<LogRecord> List(Expression<Func<LogRecord, bool>> predicate = null, uint? limit = 0) {
			using (var conn = new SQLiteConnection(_connectionString)) {
				var param = new DynamicParameters();
				string sql = ExpressionToSql.Select(predicate, param, Mapping.SelectFromRelation, x => x.Time, true, limit);
				return conn.Query<LogRecord>(sql, param);
			}
		}

		public void EnsureCreated() {
			using (var conn = new SQLiteConnection(_connectionString)) {
				conn.Execute(Mapping.CreateTableSql);
				conn.Execute(ExpressionToSql.CreateIndexOnProperty((LogRecord x) => x.Time, Mapping.SelectFromRelation));
			}
		}
	}
}
