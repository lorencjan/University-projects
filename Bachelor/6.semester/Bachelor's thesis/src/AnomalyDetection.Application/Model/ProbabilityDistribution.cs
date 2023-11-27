// File: ProbabilityDistribution.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using YSoft.Rqa.AnomalyDetection.Application.Services;

namespace YSoft.Rqa.AnomalyDetection.Application.Model
{
    /// <summary>Generates and contains data of various probability distributions.</summary>
    public class ProbabilityDistribution
    {
        /// <summary>Area around the data where a generated anomalous spike might still seem as valid.</summary>
        public Interval AnomalyRestrictions { get; }

        /// <summary>Probability distribution data.</summary>
        public List<double> Data { get; }

        /// <summary>Advised borders for natural outliers to generate to the data.</summary>
        public Interval OutlierBorders { get; }

        public ProbabilityDistribution(IEnumerable<double> data, Interval outlierBorders, Interval anomalyRestrictions) {
            Data = data.ToList();
            OutlierBorders = outlierBorders;
            AnomalyRestrictions = anomalyRestrictions;
        }

        /// <summary>Generates normal distribution according to given parameters or completely randomly.</summary>
        /// <param name="count">Number of points in the distribution.</param>
        /// <param name="mean">Mean value of the distribution. If not specified, it's chosen randomly from range 1-1000.</param>
        /// <param name="sigma">Standard deviation of the distribution. If not specified, it's chosen randomly up to 1/4 of the mean.</param>
        /// <returns>Normal distribution with precomputed outlier and anomaly borders.</returns>
        public static ProbabilityDistribution Normal(int count, double? mean = null, double? sigma = null) {
            var data = new double[count];
            mean ??= Statistics.RandDouble(1, 1000);
            sigma ??= Statistics.RandDouble(Math.Min(1, mean.Value / 4), Math.Max(1, mean.Value / 4));

            var normal = new Normal(mean.Value, sigma.Value);
            normal.Samples(data);

            var outlierBorders = new Interval(Math.Max(0, mean.Value - 5 * sigma.Value), mean.Value + 5 * sigma.Value);
            var anomalyRestrictions = new Interval(mean.Value - 3 * sigma.Value, mean.Value + 3 * sigma.Value);

            return new ProbabilityDistribution(data, outlierBorders, anomalyRestrictions);
        }

        /// <summary>Generates F-distribution according to given parameters or completely randomly.</summary>
        /// <param name="count">Number of points in the distribution.</param>
        /// <param name="center">Center of the distribution. If not specified, it's chosen randomly from range 1-1000.</param>
        /// <param name="dfNum">Degrees of freedom in numerator. If not specified, it's chosen randomly from range 5-50.</param>
        /// <param name="dfDen">Degrees of freedom in denominator. If not specified, it's chosen randomly from range 5-100.</param>
        /// <returns>F-distribution with precomputed outlier and anomaly borders.</returns>
        public static ProbabilityDistribution FDistribution(int count, double? center = null, double? dfNum = null, double? dfDen = null) {
            var data = new double[count];
            center = center == null ? Statistics.RandDouble(1, 1000) : Math.Max(center.Value, 1);
            dfNum ??= Statistics.RandDouble(5, 50);
            dfDen ??= Statistics.RandDouble(5, 100);

            var fisherSnedecor = new FisherSnedecor(dfNum.Value, dfDen.Value);
            fisherSnedecor.Samples(data);
            data = data.Select(x => x * center.Value).ToArray();

            var outlierBorders = new Interval(data.Min() * Statistics.RandDouble(0.7, 0.8), data.Max() * Statistics.RandDouble(1.2, 1.3));
            var anomalyRestrictions = new Interval(outlierBorders.Left, center.Value * Statistics.RandDouble(2, 3));

            return new ProbabilityDistribution(data, outlierBorders, anomalyRestrictions);
        }

        /// <summary>Generates triangular distribution according to given parameters or completely randomly.</summary>
        /// <param name="count">Number of points in the distribution.</param>
        /// <param name="start">Start of the triangle. If not specified, it's chosen randomly from range 1-1000.</param>
        /// <param name="end">End of the triangle. If not specified, it's chosen randomly according to the start.</param>
        /// <param name="peak">Peak of the triangle. If not specified, it's chosen randomly in the first half of the triangle.</param>
        /// <returns>Triangle distribution with precomputed outlier and anomaly borders.</returns>
        public static ProbabilityDistribution Triangular(int count, double? start = null, double? end = null, double? peak = null) {
            var data = new double[count];
            start ??= Statistics.RandDouble(1, 500);
            end ??= start * Statistics.RandDouble(1.5, 2.5);
            peak ??= start + Statistics.RandDouble((end - start).Value * 0.1, (end - start).Value * 0.4);

            var triangular = new Triangular(start.Value, end.Value, peak.Value);
            triangular.Samples(data);

            var outlierBorders = new Interval(start.Value * Statistics.RandDouble(0.8, 0.9), end.Value * Statistics.RandDouble(1.3, 1.6));
            var anomalyRestrictions = new Interval(outlierBorders.Left, start.Value + (end - start).Value * 0.9);

            return new ProbabilityDistribution(data, outlierBorders, anomalyRestrictions);
        }

        /// <summary>Generates sequence of random values from an interval.</summary>
        /// <param name="count">Number of points in the distribution.</param>
        /// <param name="interval">Interval from which to generate the distribution.</param>
        /// <returns>List of random values.</returns>
        public static List<double> Uniform(int count, Interval interval) {
            var data = new List<double>();
            for (var i = 0; i < count; i++)
                data.Add(Statistics.RandDouble(interval.Left, interval.Right));

            return data;
        }
    }
}