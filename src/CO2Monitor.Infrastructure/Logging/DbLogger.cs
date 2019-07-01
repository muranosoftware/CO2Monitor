using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Data.SQLite;
using Dapper;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CO2Monitor.Infrastructure.Logging
{
    public class DbLogger : ILogger
    {
        public const string ConnectionStringConfigurationKey = "DbLogger";

        private readonly string _categoryName;
        private readonly Func<string, LogLevel, bool> _filter;
        private const int _MessageMaxLength = 4000;
        private readonly string _connectionString;

        public DbLogger(string categoryName, Func<string, LogLevel, bool> filter, string connectionString)
        {
            _categoryName = categoryName;
            _filter = filter;
            _connectionString = connectionString;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (exception != null)
            {
                message += "\n" + exception.ToString();
            }
            message = message.Length > _MessageMaxLength ? message.Substring(0, _MessageMaxLength) : message;
            var log = new LogRecord
            {
                Message = message,
                EventId = eventId.Id,
                LogLevel = logLevel.ToString(),
                Time = DateTime.Now
            };

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Execute("INSERT INTO LogRecords(Message, EventId, LogLevel, Time) VALUES (@Message, @EventId, @LogLevel, @Time)", log);
            }

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }


        public static void Configure(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringConfigurationKey);
            var dbFile = ParseConnectionString(connectionString)["Data Source"];

            if (dbFile == ":memory:" || !File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);

                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    SQLiteCommand command = new SQLiteCommand("CREATE TABLE LogRecords (Id INTEGER, Message TEXT, EventId INTEGER, LogLevel TEXT, Time DATETIME);", conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand("CREATE INDEX LogRecordsTimeIndex ON LogRecords(Time);", conn);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var result = new Dictionary<string, string>();

            foreach (var i in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = i.Split('=', StringSplitOptions.RemoveEmptyEntries).Select( x=> x.Trim()).ToList();
                result.Add(kv[0], kv[1]);
            }

            return result;
        }


    }

}
