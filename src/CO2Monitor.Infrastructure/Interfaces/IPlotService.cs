using System;
using System.Collections.Generic;
using System.IO;

namespace CO2Monitor.Infrastructure.Interfaces {
	public struct TimeSeriesPoint {
		public double Y { get; set; }

		public DateTime Time { get; set; }

		public TimeSeriesPoint(DateTime time, double y) {
			Y = y;
			Time = time;
		}
	}

	public class TimeSeries {
		public string Name { get; set; }
		public IReadOnlyList<string> YAxisLabels { get; set; }

		public IList<TimeSeriesPoint> Data { get; set; }
	}

	public interface IPlotService {
		void Plot(string title, IEnumerable<TimeSeries> data, Stream stream, TimeSpan pollingRate);
	}
}
