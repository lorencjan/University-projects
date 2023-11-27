// File: Statistics.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace YSoft.Rqa.AnomalyDetection.Application.Services
{
    public static class Statistics
    {
        /// <summary>Computes Modified Z-Score for each data point in input dataset.</summary>
        /// <param name="arr">Input dataset.</param>
        /// <param name="median">Median parameter for Modified Z-Score. If not specified, it will be taken from the dataset.</param>
        /// <param name="mad">Median absolute deviation parameter for Modified Z-Score. If not specified, it will be taken from the dataset.</param>
        /// <returns>List of Z-Scores for each data point.</returns>
        public static List<double> ModifiedZScore(IEnumerable<double> arr, double? median = null, double? mad = null) {
            median ??= arr.Median();
            mad ??= MedianAbsoluteDeviation(arr, median);
            return arr.Select(x => 0.6745 * Math.Abs(x - median.Value) / mad.Value).ToList();
        }

        /// <summary>Computes median absolute deviation from a dataset.</summary>
        /// <param name="arr">Input dataset.</param>
        /// <param name="median">Optionally precomputed median from the dataset.</param>
        /// <returns>Median absolute deviation of the dataset.</returns>
        public static double MedianAbsoluteDeviation(IEnumerable<double> arr, double? median = null) {
            median ??= arr.Median();
            return arr.Select(x => Math.Abs(x - median.Value)).Median();
        }

        /// <summary>Estimates gaussian standard deviation from median absolute deviation.</summary>
        /// <param name="mad">Median absolute deviation to estimate from.</param>
        /// <returns>Standard deviation sigma.</returns>
        public static double EstimateStandardDeviationFromMad(double mad) => mad * 1.4826;

        /// <summary>Computes median from distances to kth neighbor in a dataset.</summary>
        /// <param name="arr">Input dataset.</param>
        /// <param name="k">Specifies to which kth neighbor to compute the distances.</param>
        /// <returns>Median from distances to kth neighbor.</returns>
        public static double MedianKthNeighborDistance(IEnumerable<double> arr, int k) {
            var sorted = arr.OrderBy(x => x).ToList();
            var kthDistances = new List<double>();
            for (var i = 0; i < sorted.Count; i++) {
                var startIdx = Math.Max(i - k, 0);
                var endIdx = Math.Min(i + k, sorted.Count - 1);
                var distances = new List<double>();
                for (var j = startIdx; j <= endIdx; j++)
                    distances.Add(Math.Abs(sorted[i] - sorted[j]));

                kthDistances.Add(distances.OrderBy(x => x).ToArray()[k]);
            }

            return kthDistances.Median();
        }

        /// <summary>Computes median from distances between a point and all other points in a dataset.</summary>
        /// <param name="arr">Input dataset.</param>
        /// <param name="point">Point whose median neighbor distance to compute.</param>
        /// <returns>Median of point's neighbor distances.</returns>
        public static double MedianNeighborDistance(IEnumerable<double> arr, double point) => arr.Select(x => Math.Abs(x - point)).Median();

        /// <summary>Generates random integer number in specified range.</summary>
        /// <param name="min">Start of the interval.</param>
        /// <param name="max">End of the interval.</param>
        /// <returns>Random integer from given interval.</returns>
        public static int RandInt(int min, int max) => new Random().Next(min, max + 1);

        /// <summary>Generates random real number in specified range.</summary>
        /// <param name="min">Start of the interval.</param>
        /// <param name="max">End of the interval.</param>
        /// <returns>Random real number from given interval.</returns>
        public static double RandDouble(double min, double max) => min + (max - min) * new Random().NextDouble();
    }
}