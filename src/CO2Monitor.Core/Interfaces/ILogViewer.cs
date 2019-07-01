using System;
using System.Collections.Generic;
using System.Text;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public interface ILogViewer
    {
        IEnumerable<LogRecord> GetRecords(DateTime? from = null, DateTime? to = null, int? limit = 1000);
    }
}
