using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using Dapper;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Data
{
    public class SQLiteDeviceStateRepository : IDeviceStateRepository
    {
        private const string configurationConnectionStringKey = "StateMeasurementsDb";
        
        private readonly string _connectionString;
        private readonly ILogger _logger;


        public SQLiteDeviceStateRepository(IConfiguration configuration, ILogger<SQLiteDeviceStateRepository> logger)
        {
            _connectionString = configuration.GetConnectionString(configurationConnectionStringKey);
            _logger = logger;
        }

      
        public void Add(DeviceStateMeasurement measurement)
        {
            using(var conn = new SQLiteConnection(_connectionString))
            {
                conn.Execute("INSERT INTO StateMeasurements (DeviceId, Time, State) VALUES (@DeviceId, @Time, @State)", measurement);
            }
        }

        public IEnumerable<DeviceStateMeasurement> List(int deviceId, DateTime? from =null, DateTime? to = null)
        {
            string sql = "SELECT * StateMeasurements WHERE DeviceId = @deviceId";
            if (from != null || to != null)
                sql += " WHERE";
            if (from != null)
                sql += " time >= @from";
            if (from != null && to != null)
                sql += " AND";
            if (to != null)
                sql += " time <= @to";

            sql += " ORDER BY time";

            using (var conn = new SQLiteConnection(_connectionString))
            {
                return conn.Query<DeviceStateMeasurement>(sql, new { deviceId, to, from});
            }
        }

        public static void Configure(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(configurationConnectionStringKey);
            var dbFile = SQLiteHelper.ParseConnectionString(connectionString)[SQLiteHelper.DataSourceKey];

            if (dbFile == SQLiteHelper.InMemorySource || !File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);

                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    SQLiteCommand command = new SQLiteCommand("CREATE TABLE StateMeasurements (Id INTEGER, DeviceId INTEGER, Time DATETIME, State Text);", conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand("CREATE INDEX StateMeasurementsDeviceIdIndex ON StateMeasurements(DeviceId); CREATE INDEX StateMeasurementsTimeIndex ON StateMeasurements(Time);", conn);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
