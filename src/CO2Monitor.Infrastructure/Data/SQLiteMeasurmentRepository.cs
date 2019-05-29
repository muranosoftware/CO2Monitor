using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Dapper;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Data
{
    public class SQLiteMeasurmentRepository : IMeasurementRepository
    {
        private const string configurationSectionKey = "SqlLite";
        private const string configurationDatabaseFileKey = "DatabaseFile";

        private static string _dbFile;
        private static string ConnectionString => $"Data Source={_dbFile};Version=3;";

        private readonly ILogger _logger;
        
        public SQLiteMeasurmentRepository(ILogger<SQLiteMeasurmentRepository> logger)
        {
            _logger = logger;
        }

        public void Add(CO2Measurement measurement)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                var sql = "INSERT INTO  Measurments (CO2, Temperature, Time) VALUES (@CO2, @Temperature, @Time)";
                conn.Execute(sql, measurement);
            }
        }

        public IList<CO2Measurement> List()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                var sql = "SELECT * FROM Measurments";
                return conn.Query<CO2Measurement>(sql).ToList();
            }
        }

        public IList<CO2Measurement> List(DateTime from, DateTime to)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                var sql = "SELECT * FROM Measurments WHERE @from <= time AND time <= @to";
                return conn.Query<CO2Measurement>(sql, new { from, to }).ToList();
            }
        }

        public static void Configure(IConfiguration configuration)
        {
            _dbFile = configuration.GetSection(configurationSectionKey)[configurationDatabaseFileKey];

            if (!File.Exists(_dbFile))
            {
                SQLiteConnection.CreateFile(_dbFile);

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    SQLiteCommand command = new SQLiteCommand("CREATE TABLE Measurments (CO2 INTEGER, Temperature REAL, Time DATETIME);", conn);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand("CREATE INDEX MeasurmentsTimeIndex ON Measurments(Time);", conn);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
