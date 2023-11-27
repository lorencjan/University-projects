// File: Clusterer.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using DBSCAN;
using DBSCAN.RBush;
using HdbscanSharp.Distance;
using HdbscanSharp.Runner;
using MathNet.Numerics.Statistics;
using Microsoft.Data.Analysis;
using YSoft.Rqa.AnomalyDetection.Application.Model;

namespace YSoft.Rqa.AnomalyDetection.Application.Services
{
    /// <summary>Takes care of any cluster manipulation.</summary>
    public class Clusterer
    {
        private const int MinClusterSizeLimit = 3;
        private const int MinCorePointLimit = 1;

        /// <summary>
        /// Performs HDBSCAN algorithm over given data.
        /// Be aware, the HDBSCAN implementation doesn't form a distance tree, but keeps the distances in a 2D matrix.
        /// This means that a very large dataset would cause out of memory error
        /// (e.g. 50 000 data points -> 50 000 * 50 000 matrix of doubles, so additionally multiplied by 8 -> 20GB).
        /// It is not an issue here as the HDBSCAN is applied only on outliers which is a small subset of the data and
        /// the detection will take place frequently enough not get to such datasets. However, it is still worth a notice.
        /// </summary>
        /// <param name="data">Input one-dimensional data to be clustered.</param>
        /// <param name="minClusterSize">Minimum number of points to be considered as a cluster.</param>
        /// <param name="k">Kth neighbor for mutual reachability distance computation.</param>
        /// <returns>List of cluster labels for each point. Clusters are >= 0, noise is -1.</returns>
        public List<int> RunHdbscan(IList<double> data, int minClusterSize, int? k = null) {
            var dataSet = new double[data.Count][];
            for (var i = 0; i < data.Count; i++)
                dataSet[i] = new[] {data[i]};

            minClusterSize = Math.Max(minClusterSize, MinClusterSizeLimit);
            if (!k.HasValue) {
                const double minPointsPercentage = 0.1;
                k = (int?) Math.Round(minClusterSize * minPointsPercentage);
            }

            var result = HdbscanRunner.Run(
                new HdbscanParameters {
                    DataSet = dataSet,
                    MinClusterSize = minClusterSize,
                    MinPoints = Math.Max(k.Value, MinCorePointLimit),
                    DistanceFunction = new EuclideanDistance(),
                    UseMultipleThread = true,
                    MaxDegreeOfParallelism = 100
                });

            return HdbscanLabelsToUniformLabels(result.Labels);
        }

        /// <summary>
        /// Labels returned by HDBSCAN algorithm are positions in cluster tree.
        /// This method converts them to a meaningful sequence.
        /// </summary>
        /// <param name="labels">Labels returned by HDBSCAN algorithm.</param>
        /// <returns>Normalized labels. Clusters are >= 0, noise is -1.</returns>
        private List<int> HdbscanLabelsToUniformLabels(int[] labels) {
            var translator = new Dictionary<int, int> {{0, -1}};
            var newLabel = 0;
            foreach (var label in labels.Distinct().Where(x => x > 0).OrderBy(x => x))
                translator[label] = newLabel++;

            return labels.Select(x => translator[x]).ToList();
        }

        /// <summary>
        /// Performs DBSCAN algorithm over given data.
        /// In comparison to HDBSCAN, it uses a distance tree so no need to worry about huge datasets.
        /// </summary>
        /// <param name="data">Input one-dimensional data to be clustered.</param>
        /// <param name="epsilon">Maximum distance for a point to be considered a neighbor.</param>
        /// <param name="minPoints">Minimum number of neighbors for a point to be considered a core point.</param>
        /// <returns>Data divided to lists for each cluster.</returns>
        public ClusterSet<DbscanPointData> RunDbscan(IEnumerable<double> data, double epsilon, int minPoints = 3) {
            var dataSet = data.Select((x, i) => new DbscanPointData(x, i)).ToList();
            minPoints = Math.Max(minPoints, MinCorePointLimit);
            return DBSCANRBush.CalculateClusters(dataSet, epsilon, minPoints);
        }

        /// <summary>
        /// DBSCAN algorithm returns sorted data but not labels.
        /// This method converts that result to a sequence of labels.
        /// </summary>
        /// <param name="clusterSet">Set of clusters returned by DBSCAN algorithm.</param>
        /// <returns>Normalized labels. Clusters are >= 0, noise is -1.</returns>
        public List<int> ClusterSetToLabels(ClusterSet<DbscanPointData> clusterSet) {
            var all = new List<DbscanPointData>(clusterSet.UnclusteredObjects);
            var label = 0;
            foreach (var cluster in clusterSet.Clusters) {
                foreach (var dataPoint in cluster.Objects)
                    dataPoint.Label = label;

                label++;
                all.AddRange(cluster.Objects);
            }

            return all.OrderBy(x => x.Index).Select(x => x.Label).ToList();
        }

        /// <summary>Divides a dataframe into smaller ones for each cluster.</summary>
        /// <param name="df">Input general dataframe.</param>
        /// <returns>List of dataframes each representing a cluster.</returns>
        public List<DataFrame> GetClustersByLabels(DataFrame df) {
            var labels = df.Get<int>(Constants.Label);
            return labels
                .Distinct()
                .Where(x => x > -1)
                .Select(label => df[labels.ElementwiseEquals(label.Value)])
                .ToList();
        }

        /// <summary>Finds the biggest/main cluster of the dataset.</summary>
        /// <param name="rg">Contains input data on which to apply clustering.</param>
        /// <returns>List of data of the biggest cluster.</returns>
        public List<double> FindMainCluster(RequestGroup rg) {
            var durations = rg.ValidData.GetAsList<double>(Constants.Duration);

            const int minPoints = 5;
            if (durations.Count < minPoints)
                return null;

            const int maxNeighbor = 5 * minPoints;
            var k = durations.Count > 2 * maxNeighbor ? maxNeighbor : (int) Math.Round(durations.Count / 2.0);
            var epsilon = Statistics.MedianKthNeighborDistance(durations, k);
            if (epsilon < 0.1) // if it's too close, it should be a cluster no matter the relativity, on the other hand, just assigning 0.1
                epsilon = Math.Min(0.1, epsilon * 10); // might make epsilon too big -> algorithm very slow ... one decimal space is enough

            var result = RunDbscan(durations, epsilon, minPoints);
            var cluster = result.Clusters.OrderByDescending(x => x.Objects.Count).FirstOrDefault();

            rg.OutlierDetectionClusters = rg.ValidData.Clone();
            var labels = new PrimitiveDataFrameColumn<int>(Constants.Label, ClusterSetToLabels(result));
            rg.OutlierDetectionClusters.Set(labels);

            return cluster?.Objects.Select(x => x.Point.X).ToList();
        }

        /// <summary>
        /// The clustering algorithms might create several neighboring clusters instead of one big cluster.
        /// This method searches for such clusters and merges them.
        /// </summary>
        /// <param name="clusters">List of clusters to potentially merge.</param>
        /// <param name="validData">Valid data for better view on what can be considered close.</param>
        /// <returns>New list of merged clusters.</returns>
        public List<DataFrame> MergeClusters(List<DataFrame> clusters, DataFrame validData) {
            var validDataWidth = validData.Get<double>(Constants.Duration).Maximum() - validData.Get<double>(Constants.Duration).Minimum();
            var toleranceDistance = 0.025 * validDataWidth; // if two clusters are closer than 2.5% of width of valid data, they are close enough
            int clustersCount;
            do {
                clustersCount = clusters.Count;
                clusters = clusters.OrderBy(x => x.Get<double>(Constants.Duration).Median()).ToList();
                var indicesToMerge = FindClusterIndicesToMerge(clusters, toleranceDistance);
                var mergedIndices = MergeClusterIndices(indicesToMerge);
                clusters = MergeClustersByIndices(clusters, mergedIndices);
            }
            while (clustersCount > clusters.Count);

            return clusters;
        }

        /// <summary>Finds which clusters are close and returns the indices of clusters to be merged.</summary>
        /// <param name="clusters">List of clusters to potentially merge.</param>
        /// <param name="toleranceDistance">A distance under which the clusters are close enough.</param>
        /// <returns>List of indices of clusters to be merged.</returns>
        private List<List<int>> FindClusterIndicesToMerge(IReadOnlyList<DataFrame> clusters, double toleranceDistance) {
            var medians = clusters.Select(x => x.Get<double>(Constants.Duration).Median()).ToList();
            var mads = clusters.Select((x, i) => Statistics.MedianAbsoluteDeviation(x.GetAsList<double>(Constants.Duration), medians[i])).ToList();
            var toBeMerged = new List<List<int>>();
            for (var i = 0; i < clusters.Count - 1; i++) {
                if ((medians[i + 1] - medians[i] < toleranceDistance || ClustersOverlap(medians[i], mads[i], medians[i + 1], mads[i + 1]))
                    && !ClusterBorderDensitiesVary(clusters[i], clusters[i + 1]))
                    toBeMerged.Add(new List<int> {i, i + 1});
            }

            return toBeMerged;
        }

        /// <summary>Merges any transient groups of cluster indices.</summary>
        /// <param name="indices">Groups of cluster indices to be merged.</param>
        /// <returns>Merged groups of cluster indices. In other words, final indication of which clusters to merge.</returns>
        private static List<List<int>> MergeClusterIndices(List<List<int>> indices) {
            for (var i = 0; i < indices.Count; i++) {
                for (var j = 0; j < indices.Count; j++) {
                    if (i >= j || !indices[i].Intersect(indices[j]).Any())
                        continue;

                    var merged = indices.Take(i).Concat(indices.Skip(i + 1).Take(j - i - 1)).Concat(indices.Skip(j + 1)).ToList();
                    merged.Add(indices[i].Union(indices[j]).ToList());
                    return MergeClusterIndices(merged);
                }
            }

            return indices;
        }

        /// <summary>Merges clusters according the merged indices.</summary>
        /// <param name="clusters">List of clusters to potentially merge.</param>
        /// <param name="mergedIndices">List of merged indices according to which the clusters should be merged.</param>
        /// <returns>New list of merged clusters.</returns>
        private static List<DataFrame> MergeClustersByIndices(List<DataFrame> clusters, List<List<int>> mergedIndices) {
            var toRemove = new List<int>();
            foreach (var indices in mergedIndices) {
                var target = indices.First();
                toRemove.AddRange(indices.Skip(1));
                foreach (var i in indices.Skip(1)) {
                    var label = clusters[target].Get<int>(Constants.Label).First();
                    clusters[i].Set(Constants.Label, label);
                    clusters[target].Append(clusters[i].Rows, true);
                }
            }

            foreach (var i in toRemove.OrderByDescending(x => x))
                clusters.RemoveAt(i);

            return clusters;
        }

        /// <summary>Checks whether two clusters are within reach from each other in respect of 3-sigma rule.</summary>
        /// <param name="leftMedian">Median of left the cluster.</param>
        /// <param name="leftMad">Median absolute deviation of the left cluster.</param>
        /// <param name="rightMedian">Median of the right cluster.</param>
        /// <param name="rightMad">Median absolute deviation of the right cluster.</param>
        /// <returns>True if the clusters are within 3-sigma from each other, false otherwise.</returns>
        private static bool ClustersOverlap(double leftMedian, double leftMad, double rightMedian, double rightMad) {
            var top = leftMedian + 3 * Statistics.EstimateStandardDeviationFromMad(leftMad);
            var bottom = rightMedian - 3 * Statistics.EstimateStandardDeviationFromMad(rightMad);
            return bottom < top;
        }

        /// <summary>Checks whether two clusters have similar densities.</summary>
        /// <param name="leftCluster">Left cluster's data.</param>
        /// <param name="rightCluster">Right cluster's data.</param>
        /// <param name="densityCoefficient">How much can one cluster be more dense than the other. Defaults to 2.5.</param>
        /// <param name="sizeLimit">Clusters under this limit are not subjected to this check. Too small to correctly determine their densities.</param>
        /// <returns>True if the clusters have similar densities, false otherwise.</returns>
        private static bool ClusterBorderDensitiesVary(DataFrame leftCluster, DataFrame rightCluster, double densityCoefficient = 2.5, int sizeLimit = 15) {
            if (leftCluster.Length() < sizeLimit || rightCluster.Length() < sizeLimit)
                return false;

            const double borderSizeCoefficient = 0.5;
            var leftClusterBorderSize = (int) Math.Max(MinClusterSizeLimit, Math.Round(leftCluster.Length() * borderSizeCoefficient));
            var rightClusterBorderSize = (int) Math.Max(MinClusterSizeLimit, Math.Round(rightCluster.Length() * borderSizeCoefficient));
            var leftClusterBorder = leftCluster.GetAsList<double>(Constants.Duration).OrderByDescending(x => x).Take(leftClusterBorderSize).ToList();
            var rightClusterBorder = rightCluster.GetAsList<double>(Constants.Duration).OrderBy(x => x).Take(rightClusterBorderSize).ToList();

            var k = Math.Min(10, (int) Math.Min(Math.Round(leftClusterBorder.Count / 2.0), Math.Round(rightClusterBorder.Count / 2.0)));
            var d1 = Statistics.MedianKthNeighborDistance(leftClusterBorder, k);
            var d2 = Statistics.MedianKthNeighborDistance(rightClusterBorder, k);
            return densityCoefficient * d1 < d2 || densityCoefficient * d2 < d1;
        }

        /// <summary>
        /// Border values of probability distributions are usually considered outliers and such a cluster might be
        /// big enough to be an anomaly. However, these are not anomalies, only not precisely detected valid/outlier data.
        /// This method checks, if the cluster neighbors with valid data and has a descending tendency in which case
        /// it contains natural outliers.
        /// </summary>
        /// <param name="cluster">Cluster to check for containing natural outliers.</param>
        /// <param name="validData">Valid data to compare against.</param>
        /// <returns>True if the cluster contains natural outliers, false otherwise.</returns>
        public bool ClusterIsFromNaturalOutliers(DataFrame cluster, DataFrame validData) {
            const int kthNeighbor = 10;
            const int clusterSizeTolerance = 15;
            const double borderZScoreLimit = 1.7;

            var clusterDurations = cluster.GetAsList<double>(Constants.Duration);
            var validDurations = validData.GetAsList<double>(Constants.Duration);
            var clusterIsRightFromValidData = clusterDurations.Median() > validDurations.Median();

            var clusterMedianDistance = clusterDurations.Select(x => Statistics.MedianNeighborDistance(clusterDurations, x)).Median();
            validDurations.Sort();
            var validDataBorderKthNeighborDistance = clusterIsRightFromValidData
                ? validDurations.Last() - validDurations[validDurations.Count - kthNeighbor]
                : validDurations[kthNeighbor] - validDurations.First();

            var distance = Math.Max(clusterMedianDistance, validDataBorderKthNeighborDistance);

            var (clusterMin, clusterMax) = (clusterDurations.Min(), clusterDurations.Max());
            if (clusterIsRightFromValidData && !validDurations.Any(x => x > clusterMin - distance) ||
                !clusterIsRightFromValidData && !validDurations.Any(x => x < clusterMax + distance))
                return false;

            var clusterWidth = clusterMax - clusterMin;
            var validRightBorder = validDurations.Max() - clusterWidth;
            var validLeftBorder = validDurations.Min() + clusterWidth;
            var borderValidDataCount = clusterIsRightFromValidData
                ? validDurations.Count(x => x > validRightBorder)
                : validDurations.Count(x => x < validLeftBorder);

            // if the cluster is significantly bigger than the valid border, spike is right there
            if (2.5 * borderValidDataCount < clusterDurations.Count)
                return false;

            if (clusterDurations.Count < clusterSizeTolerance)
                return true;

            var zScores = Statistics.ModifiedZScore(clusterDurations.OrderBy(x => x));
            var borderZScore = clusterIsRightFromValidData ? zScores.First() : zScores.Last();
            return borderZScore < borderZScoreLimit;
        }
    }
}