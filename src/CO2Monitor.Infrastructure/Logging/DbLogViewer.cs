using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Dapper;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Logging
{
    public class DbLogViewer : ILogViewer
    {
        private readonly string _connString;

        public DbLogViewer(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString(DbLogger.ConnectionStringConfigurationKey);
        }

        public IEnumerable<LogRecord> GetRecords(DateTime? from = null, DateTime? to = null, int? limit = 100)
        {
            string sql = "SELECT * FROM LogRecords";
            if (from != null || to != null)
                sql += " WHERE";
            if (from != null)
                sql += " time >= @from";
            if (from != null && to != null)
                sql += " AND";
            if (to != null)
                sql += " time <= @to";

            sql += " ORDER BY time DESC";

            if (limit != null)
                sql += $" LIMIT {limit}";

            using (var conn = new SQLiteConnection(_connString))
            {
                return conn.Query<LogRecord>(sql, new { from, to});
            }
        }
    }
}
