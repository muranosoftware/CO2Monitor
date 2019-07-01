using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Entities
{ 
    public partial class LogRecord
    {
        public int Id { get; set; }
        public int? EventId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public DateTime? Time { get; set; }
    }
}
