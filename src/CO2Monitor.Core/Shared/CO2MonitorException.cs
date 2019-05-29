using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Shared
{
    public class CO2MonitorException : Exception
    {
        public CO2MonitorException()
        {
        }

        public CO2MonitorException(string message) : base(message)
        {
        }

        public CO2MonitorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
