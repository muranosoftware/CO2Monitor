using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Dapper;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Data
{
	public class SqLiteDeviceStateRepository : IDeviceStateRepository {
		public const string DataSourceConfigurationKey = "SqLiteDeviceStateRepository:DataSource";

		public string DataSource { get; }

		private readonly string _connectionString;

		private static readonly SqLiteTableMapping<DeviceStateMeasurement> Mapping = new SqLiteTableMapping<DeviceStateMeasurement>("Measurements");

		public SqLiteDeviceStateRepository(IConfiguration configuration) {
			DataSource = configuration.GetValue<string>(DataSourceConfigurationKey);
			_connectionString = $"Data Source={DataSource};";
		}

		public void Add(DeviceStateMeasurement measurement) {
			using (var conn = new SQLiteConnection(_connectionString))
			{
				conn.Execute(Mapping.CreateSql, measurement);
			}
		}

		public IEnumerable<DeviceStateMeasurement> List(Expression<Func<DeviceStateMeasurement, bool>> predicate = null) {
			using (var conn = new SQLiteConnection(_connectionString)) {
				var param = new DynamicParameters();
				string sql = ExpressionToSql.Select(predicate, param, Mapping.SelectFromRelation, x => x.Time);
				return conn.Query<DeviceStateMeasurement>(sql, param);
			}
		}

		public void EnsureCreated() {
			using (var conn = new SQLiteConnection(_connectionString)) {
				conn.Execute(Mapping.CreateTableSql);
				conn.Execute(ExpressionToSql.CreateIndexOnProperty((DeviceStateMeasurement x) => x.Time, Mapping.SelectFromRelation));
				conn.Execute(ExpressionToSql.CreateIndexOnProperty((DeviceStateMeasurement x) => x.DeviceId, Mapping.SelectFromRelation));
			}
		}
	}
}
