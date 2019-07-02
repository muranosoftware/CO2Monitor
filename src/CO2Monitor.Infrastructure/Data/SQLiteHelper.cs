using System;
using System.Collections.Generic;
using System.Linq;

namespace CO2Monitor.Infrastructure.Data
{
    public static class SQLiteHelper
    {
        public const string DataSourceKey = "Data Source";
        public const string InMemorySource = ":memory:";

        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var result = new Dictionary<string, string>();

            foreach (var i in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = i.Split('=', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                result.Add(kv[0], kv[1]);
            }

            return result;
        }
    }
}
