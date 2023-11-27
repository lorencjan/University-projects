// File: TrafficGenerator.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using YSoft.Rqa.AnomalyDetection.Application.Model;
using YSoft.Rqa.AnomalyDetection.Data.Model.Csv;
using YSoft.Rqa.AnomalyDetection.Data.Model.Graylog;
using YSoft.Rqa.AnomalyDetection.Data.Services;

namespace YSoft.Rqa.AnomalyDetection.Application.Services
{
    /// <summary>Handles mocking RQA network traffic.</summary>
    public class TrafficGenerator
    {
        /// <summary>Generates completely random traffic symbolizing a service request type.</summary>
        /// <param name="interval">Interval in which the traffic takes place.</param>
        /// <param name="count">Number of valid requests to generate (additional outlier or anomalies are added to this number).</param>
        /// <param name="countIsFixed">If true, exactly <paramref name="count" /> will be generated, otherwise it's a random number from 100 to <paramref name="count" /></param>
        /// <param name="saveToLocation">If specified, the traffic will be saved to a file as csv.</param>
        /// <returns></returns>
        public List<RequestDataPoint> GenerateRequests(DateTimeInterval interval = null, int count = 5000, bool countIsFixed = false, string saveToLocation = null) {
            if (!countIsFixed)
                count = Statistics.RandInt(100, Math.Max(100, count));

            var timestamps = GenerateRequestTimestamps(count, interval);
            var durations = GenerateRequestDurations(timestamps.Count);
            var errors = GenerateRequestErrors(timestamps.Count);

            var requests = Enumerable
                .Range(0, count)
                .Select(i => new RequestDataPoint {Timestamp = timestamps[i], Duration = durations[i], Errors = errors[i]})
                .ToList();

            if (!string.IsNullOrEmpty(saveToLocation)) {
                var csvHandler = new CsvHandler();
                csvHandler.WriteToCsv(saveToLocation, requests);
            }

            return requests;
        }

        /// <summary>
        /// Randomly generates request durations. Firstly, it generates valid data from randomly chosen probability distribution.
        /// Secondly, it adds some random outliers, 0.5-3% of the amount valid ones looks reasonable.
        /// Finally, it decides on number of anomalies. With 10% there will be none, another 10% will be two, 80% for one anomaly.
        /// It than generates that many anomalies again from random distributions in a way the anomaly is not close enough to
        /// valid data to be considered valid as well. That might still occur, the probability is very low though.
        /// </summary>
        /// <param name="count">Number of valid data to be generated.</param>
        /// <returns>List of random durations containing valid, outlier and anomaly data.</returns>
        public List<double> GenerateRequestDurations(int count) {
            var distributionType = Statistics.RandInt(0, 2);
            var outlierCount = (int) (Statistics.RandDouble(0.005, 0.03) * count);
            var anomalyGen = Statistics.RandDouble(0, 1);
            var numOfAnomalies = anomalyGen <= 0.1 ? 0 : anomalyGen >= 0.9 ? 2 : 1;

            var distribution = distributionType switch {
                0 => ProbabilityDistribution.Normal(count),
                1 => ProbabilityDistribution.FDistribution(count),
                2 => ProbabilityDistribution.Triangular(count),
                _ => throw new NotImplementedException()
            };

            var randomOutliers = ProbabilityDistribution.Uniform(outlierCount, distribution.OutlierBorders);

            var availableSpaces = new List<Interval>();
            if (distribution.OutlierBorders.Left < distribution.AnomalyRestrictions.Left)
                availableSpaces.Add(new Interval(distribution.OutlierBorders.Left, distribution.AnomalyRestrictions.Left));

            if (distribution.AnomalyRestrictions.Right < distribution.OutlierBorders.Right)
                availableSpaces.Add(new Interval(distribution.AnomalyRestrictions.Right, distribution.OutlierBorders.Right));

            if (!availableSpaces.Any())
                return distribution.Data.Concat(randomOutliers).Shuffle().ToList();

            var anomalies = new List<List<double>>();
            for (var i = 0; i < numOfAnomalies; i++) {
                var size = (int) Math.Max(5, Statistics.RandDouble(0.075, 0.125) * (count + outlierCount));
                var space = availableSpaces[Statistics.RandInt(0, availableSpaces.Count - 1)];
                var baseRange = new Interval(space.Left + 0.2 * space.Length, space.Right - 0.5 * space.Length);
                var anomalyBase = Statistics.RandDouble(baseRange.Left, baseRange.Right);

                var anomalyDistributionType = Statistics.RandInt(0, 2);
                switch (anomalyDistributionType) {
                    case 0:
                        var sigma = Math.Min(anomalyBase - space.Left, space.Right - anomalyBase) * Statistics.RandDouble(0.1, 0.2);
                        anomalies.Add(ProbabilityDistribution.Normal(size, anomalyBase, sigma).Data);
                        break;
                    case 1:
                        var width = Statistics.RandDouble(anomalyBase * 1.1, Math.Min(space.Right, anomalyBase * 1.3));
                        anomalies.Add(ProbabilityDistribution.Triangular(size, anomalyBase, width).Data);
                        break;
                    case 2:
                        var interval = new Interval(anomalyBase, (space.Right - anomalyBase) * Statistics.RandDouble(0.05, 0.15));
                        anomalies.Add(ProbabilityDistribution.Uniform(size, interval));
                        break;
                }
            }

            return distribution.Data
                .Concat(randomOutliers)
                .Concat(anomalies.SelectMany(x => x))
                .Shuffle()
                .ToList();
        }

        /// <summary>Randomly generates datetime timestamps from an interval.</summary>
        /// <param name="count">Number of timestamps to generate.</param>
        /// <param name="interval">Interval from which to generate the timestamps.</param>
        /// <returns>List of random timestamp in ascending order.</returns>
        public List<DateTime> GenerateRequestTimestamps(int count, DateTimeInterval interval) {
            interval ??= new DateTimeInterval(DateTime.Now.AddDays(-1), DateTime.Now);
            var top = (int) Math.Round(interval.Length.TotalMilliseconds);
            var timestamps = new List<DateTime>();
            for (var i = 0; i < count; i++)
                timestamps.Add(interval.Start.AddMilliseconds(Statistics.RandInt(0, top)));

            return timestamps.OrderBy(x => x).ToList();
        }

        /// <summary>Randomly generates number of errors (0-2) in requests.</summary>
        /// <param name="count">Number of error counts to generate.</param>
        /// <returns>List of error counts for each request.</returns>
        public List<int> GenerateRequestErrors(int count) {
            var maxNumOfErrors = Statistics.RandInt(0, 2);
            switch (maxNumOfErrors) {
                case 1:
                {
                    var noErrorCount = (int) (Statistics.RandDouble(0, 1) * count);
                    return Enumerable.Repeat(0, noErrorCount)
                        .Concat(Enumerable.Repeat(1, count - noErrorCount))
                        .Shuffle()
                        .ToList();
                }
                case 2:
                {
                    var border1 = Statistics.RandDouble(0, 0.75);
                    var border2 = Statistics.RandDouble(border1, 1);
                    var noErrorCount = (int) (border1 * count);
                    var oneErrorCount = (int) ((border2 - border1) * count);
                    var twoErrorsCount = count - noErrorCount - oneErrorCount;
                    return Enumerable.Repeat(0, noErrorCount)
                        .Concat(Enumerable.Repeat(1, oneErrorCount))
                        .Concat(Enumerable.Repeat(2, twoErrorsCount))
                        .Shuffle()
                        .ToList();
                }
                default:
                    return Enumerable.Repeat(0, count).ToList();
            }
        }
    }
}