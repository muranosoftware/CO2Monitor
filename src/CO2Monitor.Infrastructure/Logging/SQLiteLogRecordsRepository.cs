﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CO2Monitor.Core.Entities;
using CO2Monitor.Infrastructure.Data;
using Dapper;
using System.Data.SQLite;


namespace CO2Monitor.Infrastructure.Logging {
	public class SqLiteLogRecordsRepository : ILogRecordsRepository {
		private const string DbFile = "EventLog.sqlite";

		private const string ConnectionString = "Data Source=" + DbFile + ";";

		private static readonly SqLiteTableMapping<LogRecord> Mapping = new SqLiteTableMapping<LogRecord>("LogRecords");

		public void Add(LogRecord measurement) {
			using (var conn = new SQLiteConnection(ConnectionString)) {
				conn.Execute(Mapping.CreateSql, measurement);
			}
		}

		public IEnumerable<LogRecord> List(Expression<Func<LogRecord, bool>> predicate = null, uint? limit = 0) {
			using (var conn = new SQLiteConnection(ConnectionString)) {
				var param = new DynamicParameters();
				string sql = ExpressionToSql.Select(predicate, param, Mapping.SelectFromRelation, x => x.Time, true, limit);
				return conn.Query<LogRecord>(sql, param);
			}
		}

		public void EnsureCreated() {
			using (var conn = new SQLiteConnection(ConnectionString)) {
				conn.Execute(Mapping.CreateTableSql);
				conn.Execute(ExpressionToSql.CreateIndexOnProperty((LogRecord x) => x.Time, Mapping.SelectFromRelation));
			}
		}
	}
}
