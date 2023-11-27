// File: Plotter.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Microsoft.Data.Analysis;
using XPlot.Plotly;
using YSoft.Rqa.AnomalyDetection.Application.Model;

namespace YSoft.Rqa.AnomalyDetection.Application.Services
{
    /// <summary>Takes care of plotting the anomaly detection results.</summary>
    public class Plotter
    {
        private const string ValidColor = "#00cc00";
        private const string OutlierColor = "#707070";
        private const string AnomalyColor = "#ff0000";

        private const string TitleDurations = "Request durations [ms]";
        private const string TitleTimestamps = "Timestamps";
        private const string TitleCount = "Count";
        private const string TitleErrorPercentages = "Amount of requests [%]";
        private const string TitleErrorCounts = "Error counts";

        private const string LegendValidData = "Valid data";
        private const string LegendOutliers = "Outliers";
        private const string LegendAnomalies = "Anomalies";
        private const string LegendInputData = "Input data";
        private const string LegendReferenceData = "Reference data";

        private readonly string[] _colorPalette = {"#7cb5ec", "#90ed7d", "#f7a35c", "#815c80", "#91e8e1", "#70cc70", "#f9e076", "#ffc0cb", "#2b908f", "#d6b85a"};

        /// <summary>Creates a scatter plot for request durations from a single dataset.</summary>
        /// <param name="df">Requests dataset.</param>
        /// <param name="title">Optional title for the plot.</param>
        /// <returns>Scatter plot chart ready to be rendered.</returns>
        public PlotlyChart Scatter(DataFrame df, string title = null) {
            var labels = df.Get<int>(Constants.Label);
            var colors = labels.Any(x => x == null)
                ? Enumerable.Repeat(_colorPalette.First(), df.Length())
                : labels.Select(label => label < 0 ? OutlierColor : _colorPalette[label.Value % _colorPalette.Length]);

            var chart = Chart.Plot(CreateSingleScatterPlot(df, colors));
            chart.WithTitle(title);
            chart.WithXTitle(TitleTimestamps);
            chart.WithYTitle(TitleDurations);
            return chart;
        }

        /// <summary>Creates a request duration class for a scatter plot.</summary>
        /// <param name="df">Dataset from which the request durations should be plotted.</param>
        /// <param name="color">Color(s) of the class.</param>
        /// <param name="name">Name of the chart</param>
        /// <returns>Single scatter chart for a scatter plot</returns>
        private Graph.Scatter CreateSingleScatterPlot(DataFrame df, object color, string name = null) {
            var timestamps = df.Get<DateTime>(Constants.Timestamp);
            var durations = df.Get<double>(Constants.Duration);
            return new Graph.Scatter {x = timestamps, y = durations, name = name, mode = "markers", marker = Marker(color)};
        }

        /// <summary>Creates a scatter plot for request durations from the anomaly detection result.</summary>
        /// <param name="rg">Requests data object.</param>
        /// <param name="title">Optional title for the plot.</param>
        /// <returns>Scatter plot chart containing valid data, outliers and anomalies.</returns>
        public PlotlyChart DetectionScatter(RequestGroup rg, string title = null) {
            var chart = Chart.Plot(
                new List<Graph.Scatter> {
                    CreateSingleScatterPlot(rg.ValidData, ValidColor, LegendValidData),
                    CreateSingleScatterPlot(rg.Outliers, OutlierColor, LegendOutliers),
                    CreateSingleScatterPlot(rg.Anomalies, AnomalyColor, LegendAnomalies)
                });

            chart.WithTitle(title);
            chart.WithXTitle(TitleTimestamps);
            chart.WithYTitle(TitleDurations);
            return chart;
        }

        /// <summary>Creates a histogram plot for request durations from a single dataset.</summary>
        /// <param name="df">Requests dataset.</param>
        /// <param name="numOfBins">Number of bins in the histogram.</param>
        /// <param name="title">Optional title for the plot.</param>
        /// <returns>Histogram plot chart ready to be rendered.</returns>
        public PlotlyChart Histogram(DataFrame df, int numOfBins = 50, string title = null) {
            var chart = Chart.Plot(new Graph.Histogram {x = df.Get<double>(Constants.Duration), nbinsx = numOfBins});
            chart.WithLayout(new Layout.Layout {title = title, bargap = 0.1});
            chart.WithXTitle(TitleDurations);
            chart.WithYTitle(TitleCount);
            return chart;
        }

        /// <summary>Creates a histogram plot for request durations from the anomaly detection result.</summary>
        /// <param name="rg">Requests data object.</param>
        /// <param name="numOfBins">Number of bins in the histogram.</param>
        /// <param name="title">Optional title for the plot.</param>
        /// <returns>Histogram plot chart containing valid data, outliers and anomalies.</returns>
        public PlotlyChart DetectionHistogram(RequestGroup rg, int numOfBins = 50, string title = null) {
            var validData = rg.ValidData.Get<double>(Constants.Duration);
            var outliers = rg.Outliers.Get<double>(Constants.Duration);
            var anomalies = rg.Anomalies.Get<double>(Constants.Duration);

            var min = new List<double> {validData.Minimum(), outliers.Minimum(), anomalies.Minimum()}.Where(x => !double.IsNaN(x)).Min();
            var max = new List<double> {validData.Maximum(), outliers.Maximum(), anomalies.Maximum()}.Where(x => !double.IsNaN(x)).Max();
            var xBins = new Graph.Xbins {start = min, end = max, size = (max - min) / numOfBins};

            var histograms = new List<Graph.Histogram> {
                new Graph.Histogram {x = validData, name = LegendValidData, xbins = xBins, marker = Marker(ValidColor)},
                new Graph.Histogram {x = outliers, name = LegendOutliers, xbins = xBins, marker = Marker(OutlierColor)},
                new Graph.Histogram {x = anomalies, name = LegendAnomalies, xbins = xBins, marker = Marker(AnomalyColor)}
            };

            var chart = Chart.Plot(histograms);
            chart.WithLayout(new Layout.Layout {title = title, barmode = "stack", bargap = 0.1});

            chart.WithXTitle(TitleDurations);
            chart.WithYTitle(TitleCount);
            return chart;
        }

        /// <summary>Creates a bar plot for request error counts in a dataset.</summary>
        /// <param name="df">Dataset from which the error counts should be plotted.</param>
        /// <param name="refDf">Optional reference dataset against which the error counts are compared.</param>
        /// <param name="title">Optional title for the plot.</param>
        /// <returns>Bar plot chart showing the request error count.</returns>
        public PlotlyChart ErrorBar(DataFrame df, DataFrame refDf = null, string title = null) {
            var plots = new List<Graph.Bar> {CreateSingleErrorBarPlot(df, LegendInputData)};

            if (refDf != null)
                plots.Add(CreateSingleErrorBarPlot(refDf, LegendReferenceData));

            var chart = Chart.Plot(plots);
            chart.WithTitle(title);
            chart.WithXTitle(TitleErrorCounts);
            chart.WithYTitle(TitleErrorPercentages);

            return chart;
        }

        /// <summary>Creates an error class for a bar plot.</summary>
        /// <param name="df">Dataset from which the error counts should be plotted.</param>
        /// <param name="name">Name of the chart</param>
        /// <returns>Single bar chart for a bar plot</returns>
        private Graph.Bar CreateSingleErrorBarPlot(DataFrame df, string name) {
            var groupedDf = df.GroupBy(Constants.Errors).Count().OrderBy(Constants.Errors);
            var errors = groupedDf.Get<int>(Constants.Errors);
            var errorCountsInPercents = groupedDf.Get<long>(Constants.Timestamp).Select(x => Math.Round(100.0 * x.Value / df.Length(), 2));
            return new Graph.Bar {name = name, x = errors, y = errorCountsInPercents, legendgroup = name};
        }

        private Graph.Marker Marker(object color) => new Graph.Marker {color = color};
    }
}