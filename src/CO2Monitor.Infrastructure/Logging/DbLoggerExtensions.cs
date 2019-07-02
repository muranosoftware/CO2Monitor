using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CO2Monitor.Infrastructure.Logging
{
    public static class DbLoggerExtensions
    {
        public static ILoggerFactory AddDbContext(this ILoggerFactory factory,
        Func<string, LogLevel, bool> filter = null, string connectionStr = null)
        {
            factory.AddProvider(new DbLoggerProvider(filter, connectionStr));
            return factory;
        }

        public static ILoggerFactory AddDbContext(this ILoggerFactory factory, LogLevel minLevel, string connectionStr)
        {
            return AddDbContext( factory, (_, logLevel) => logLevel >= minLevel, connectionStr);
        }
    }
}
