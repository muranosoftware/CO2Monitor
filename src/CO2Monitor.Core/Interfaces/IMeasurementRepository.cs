using System;
using System.Collections.Generic;
using System.Text;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    public interface IMeasurementRepository
    {
        IList<CO2Measurement> List();

        IList<CO2Measurement> List(DateTime from, DateTime to);

        void Add(CO2Measurement measurement);
    }
}