using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Entities
{
    public class CO2Measurement
    {
        public int CO2 { get; set; }

        public float Temperature { get; set; }

        public DateTime Time { get; set; }
    }
}
