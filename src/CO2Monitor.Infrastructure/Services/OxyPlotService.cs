using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Core.Drawing;
using OxyPlot.Series;
using CO2Monitor.Core.Interfaces.Services;

namespace CO2Monitor.Infrastructure.Services {
	public class OxyPlotService : IPlotService {
		const int Width = 360;
		const int Height = 300;

		static readonly OxyColor[] ColorMap = new[] { 
			// MATLAB colormap
			OxyColor.FromRgb(000, 114, 189),
			OxyColor.FromRgb(217, 083, 025),
			OxyColor.FromRgb(237, 177, 032),
			OxyColor.FromRgb(126, 047, 142),
			OxyColor.FromRgb(119, 172, 048),
			OxyColor.FromRgb(077, 190, 238),
			OxyColor.FromRgb(162, 020, 047)
		};

		public void Plot(string title, IEnumerable<TimeSeries> data, Stream stream, TimeSpan pollingRate) {
			var model = new PlotModel { Title = $"{title} : {data.Aggregate("", (acc, s) => acc + " " + s.Name)}"};

			model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom });
			var rnd = new Random(); 

			int i = rnd.Next(ColorMap.Length);
			foreach (TimeSeries s in data) {
				i %= ColorMap.Length;
				OxyColor color = ColorMap[i];

				Axis axis;
				if (s.YAxisLabels == null) {
					axis = new LinearAxis {
						//Title = s.Name,
						AxislineColor = color,
						MajorGridlineColor = color,
						MinorGridlineColor = color,
						TicklineColor = color,
						TextColor = color,
						Position = AxisPosition.Left,
						PositionTier = i + 1,
						Key = s.Name,
						IsAxisVisible = true,
					};
				} else {
					var categoryAxis = new CategoryAxis {
						AxislineColor = color,
						MajorGridlineColor = color,
						MinorGridlineColor = color,
						TicklineColor = color,
						TextColor = color,
						Position = AxisPosition.Left,
						PositionTier = i + 1,
						Key = s.Name,
						IsAxisVisible = true,
						IsTickCentered = true,
					};

					categoryAxis.ActualLabels.AddRange(s.YAxisLabels);
					axis = categoryAxis;
				}

				var series = new LineSeries {
					Title = s.Name,
					YAxisKey = s.Name,
					Color = color
				};

				series.Points.Capacity = s.Data.Count;
				if (s.Data.Count >= 2) {
					series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(s.Data[0].Time), s.Data[0].Y));
					for (int t = 1; t < s.Data.Count; t++) {
						TimeSpan dt = s.Data[t].Time - s.Data[t - 1].Time;

						if (dt >= pollingRate * 2) {
							series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(s.Data[t].Time + (0.5 * dt)), double.NaN));
						}

						series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(s.Data[t].Time), s.Data[t].Y));
					}
				}

				model.Axes.Add(axis);
				model.Series.Add(series);
				i++;
			}

			PngExporter.Export(model, stream, Width, Height, OxyColors.White);

			stream.Flush();
			stream.Position = 0;
		}
	}
}
