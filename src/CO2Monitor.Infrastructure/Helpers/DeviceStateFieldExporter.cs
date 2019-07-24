using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Helpers {
	public static class DeviceStateFieldExporter {
		public static IEnumerable<TimeSeriesPoint> GetFloatFieldValues(IEnumerable<DeviceStateMeasurement> measurements, string field) {
			foreach (DeviceStateMeasurement m in measurements) {
				JObject json = JObject.Parse(m.State);
				if (json.ContainsKey(field))
					yield return new TimeSeriesPoint(m.Time, json.Property(field).ToObject<double>());
			}
		}

		public static IEnumerable<TimeSeriesPoint> GetEnumFieldValues(IEnumerable<DeviceStateMeasurement> measurements, string field, IReadOnlyList<string> enumValues) {
			var indexes = new Dictionary<string, double>();

			for (var i = 0; i < enumValues.Count; i++) {
				indexes.Add(enumValues[i].ToLower(), i);
				indexes.Add(enumValues[i].ToUpper(), i);
			}

			indexes.Add(string.Empty, -1);

			foreach (DeviceStateMeasurement m in measurements) {
				JObject json = JObject.Parse(m.State);
				if (json.ContainsKey(field))
					yield return new TimeSeriesPoint(m.Time, indexes[json.Property(field).ToObject<string>()]);
			}
		}
	}
}
