// File: DurationAnomalyDetector.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Microsoft.Data.Analysis;
using YSoft.Rqa.AnomalyDetection.Application.Model;

namespace YSoft.Rqa.AnomalyDetection.Application.Services
{
    /// <summary>Provides operations for outlier and anomaly detection in request durations.</summary>
    public class DurationAnomalyDetector
    {
        private readonly Clusterer _clusterer;

        public DurationAnomalyDetector(Clusterer clusterer) {
            _clusterer = clusterer;
        }

        /// <summary>
        /// Finds outliers using combination of DBSCAN and Modified Z-Score.
        /// It uses either provided reference data or the biggest cluster found to obtain
        /// Modified Z-score parameters, which is than applied to detect outliers.
        /// </summary>
        /// <param name="rg">Contains input data on which to apply the outlier detection.</param>
        /// <param name="referenceData">Optional reference data for Modified Z-Score parameters.</param>
        public void FindOutliers(RequestGroup rg, DataFrame referenceData = null) {
            var durations = referenceData == null
                ? _clusterer.FindMainCluster(rg)
                : referenceData.GetAsList<double>(Constants.Duration);

            if (durations == null || durations.Count == 0) {
                if (rg.ValidData.Length() > 3)
                    (rg.ValidData, rg.Outliers) = RunModifiedZScore(rg.ValidData);

                return;
            }

            var median = durations.Median();
            var mad = Statistics.MedianAbsoluteDeviation(durations, median);

            (rg.ValidData, rg.Outliers) = RunModifiedZScore(rg.ValidData, median, mad);
        }

        /// <summary>Applies Modified Z-Score on the dataset.</summary>
        /// <param name="df">Input data on which to apply the method.</param>
        /// <param name="median">Median parameter for Modified Z-Score. If not specified, it will be taken from the dataset.</param>
        /// <param name="mad">Median absolute deviation parameter for Modified Z-Score. If not specified, it will be taken from the dataset.</param>
        /// <param name="zScoreLimit">Z-score value over which a data point is considered an outlier.</param>
        /// <returns>Original dataset divided on valid data and outliers.</returns>
        private (DataFrame, DataFrame) RunModifiedZScore(DataFrame df, double? median = null, double? mad = null, double zScoreLimit = 2.35) {
            var durations = df.GetAsList<double>(Constants.Duration);
            median ??= durations.Median();
            mad ??= Statistics.MedianAbsoluteDeviation(durations, median);
            var zScores = Statistics.ModifiedZScore(durations, median, mad);

            var zScoresCol = new PrimitiveDataFrameColumn<double>(Constants.ZScore, zScores);
            df.Set(zScoresCol);
            var filter = zScoresCol.ElementwiseLessThanOrEqual(zScoreLimit);
            var validData = df[filter];
            var outliers = df[filter.Xor(true)];
            return (validData, outliers);
        }

        /// <summary>
        /// Detects clusters in outliers and the ones big enough proclaims anomalies.
        /// Firstly, it clusters the data, than merges clusters which are close to each other.
        /// Secondly, it smooths the clusters using Modified Z-Score.
        /// Lastly, it checks for each cluster if it's not from natural outliers of the data and if it's big enough.
        /// </summary>
        /// <param name="rg">Contains input data with already detected outliers on which to apply anomaly detection.</param>
        /// <param name="referenceData">Optional reference data to stress the valid data.</param>
        /// <param name="anomalyLimit">Fraction of total amount of input data specifying minimum anomaly size. Defaults to 5% of total data count.</param>
        /// <param name="tolerance">Defines how far from valid data in milliseconds a potential anomaly is not considered as an anomaly.</param>
        public void FindAnomalies(RequestGroup rg, DataFrame referenceData = null, double anomalyLimit = 0.05, double tolerance = 0) {
            if (rg.Outliers.Length() == 0)
                return;

            var validData = rg.ValidData.Clone();
            if (referenceData != null)
                validData.Append(referenceData.Rows, true);

            var outlierGroups = SeparateOutliers(rg);
            var outlierClusters = ClusterOutliers(outlierGroups, validData, rg);
            var outliers = CorrectOutliersAfterClustering(ref outlierClusters, rg);
            SaveClustersForPlot(outlierClusters, outliers, rg);
            DecideForAnomalies(outlierClusters, validData, rg, anomalyLimit, tolerance);

            rg.Anomalies = rg.Anomalies.DropDuplicates<DateTime>(Constants.Timestamp);
            rg.Outliers = rg.Outliers.Append(rg.Anomalies.Rows).DropDuplicates<DateTime>(Constants.Timestamp, false);
        }

        /// <summary>To prevent clustering of outliers over valid data, this method separates the outliers by the valid data.</summary>
        /// <param name="rg">Contains input data with already detected outliers.</param>
        /// <returns>List of separated outlier groups. Basically, there are just 2 groups, one from each side.</returns>
        private List<DataFrame> SeparateOutliers(RequestGroup rg) {
            var validDurations = rg.ValidData.Get<double>(Constants.Duration);
            var border = validDurations.Median();
            var outlierDurations = rg.Outliers.Get<double>(Constants.Duration);
            return new List<DataFrame> {rg.Outliers[outlierDurations.ElementwiseLessThan(border)], rg.Outliers[outlierDurations.ElementwiseGreaterThan(border)]};
        }

        /// <summary>
        /// Applies DBSCAN on small datasets or HDBSCAN on bigger to create the clusters.
        /// It than merges close clusters as both algorithms can create many smaller ones instead of a correct one.
        /// </summary>
        /// <param name="outlierGroups">Outlier groups on which to apply clustering.</param>
        /// <param name="validData">Valid data for reference purposes.</param>
        /// <param name="rg">The input data object for saving middle steps.</param>
        /// <returns>List of outlier clusters.</returns>
        private List<DataFrame> ClusterOutliers(IEnumerable<DataFrame> outlierGroups, DataFrame validData, RequestGroup rg) {
            const double minClusterPercentage = 0.05;
            const int smallClusterLimit = 30;
            var lastLabel = -1;
            var outlierClusters = new List<DataFrame>();
            foreach (var group in outlierGroups) {
                var size = group.Length();
                if (size == 0)
                    continue;

                var dataSet = group.GetAsList<double>(Constants.Duration);
                List<int> labels;
                if (size > smallClusterLimit) {
                    var minClusterSize = (int) Math.Round(size * minClusterPercentage);
                    labels = _clusterer.RunHdbscan(dataSet, minClusterSize);
                }
                else {
                    var validDurations = validData.Get<double>(Constants.Duration);
                    var maxDistance = (validDurations.Maximum() - validDurations.Minimum()) / 4;
                    var naturalDistance = Statistics.MedianKthNeighborDistance(dataSet, (int) Math.Round(dataSet.Count / 2.0));
                    var epsilon = Math.Min(maxDistance, naturalDistance);
                    var clusterSet = _clusterer.RunDbscan(dataSet, epsilon);
                    labels = _clusterer.ClusterSetToLabels(clusterSet);
                }

                labels = labels.Select(x => x == -1 ? -1 : x + lastLabel + 1).ToList();
                lastLabel = labels.Max();
                group.Set(new PrimitiveDataFrameColumn<int>(Constants.Label, labels));

                rg.AnomalyDetectionClusters = rg.AnomalyDetectionClusters == null
                    ? group.Clone()
                    : rg.AnomalyDetectionClusters.Append(group.Rows);

                var clusters = _clusterer.GetClustersByLabels(group);
                outlierClusters.AddRange(_clusterer.MergeClusters(clusters, validData));
            }

            return outlierClusters;
        }

        /// <summary>
        /// As smaller clusters were merged, there could have been outliers between them.
        /// This method corrects it by putting these into the merged cluster.
        /// </summary>
        /// <param name="outlierClusters">Reference to the list of outlier clusters which is to be modified.</param>
        /// <param name="rg">The input data object for reference purposes.</param>
        /// <returns>Outliers not belonging to any cluster.</returns>
        private DataFrame CorrectOutliersAfterClustering(ref List<DataFrame> outlierClusters, RequestGroup rg) {
            var outliers = rg.AnomalyDetectionClusters[rg.AnomalyDetectionClusters.Get<int>(Constants.Label).ElementwiseEquals(-1)];
            foreach (var cluster in outlierClusters) {
                var clusterDurations = cluster.Get<double>(Constants.Duration);
                var (min, max) = (clusterDurations.Minimum(), clusterDurations.Maximum());
                var filter = new PrimitiveDataFrameColumn<bool>("filter", outliers.Get<double>(Constants.Duration).Select(x => x > min && x < max));
                var outliersInsideCluster = outliers[filter];
                outliersInsideCluster.Set(Constants.Label, cluster.Get<int>(Constants.Label).First());
                cluster.Append(outliersInsideCluster.Rows, true);
                outliers = outliers[filter.Xor(true)];
            }

            return outliers;
        }

        /// <summary>Saves the anomaly detection state for plotting purposes.</summary>
        /// <param name="outlierClusters">Outliers belonging to a cluster.</param>
        /// <param name="outliers">Outliers belonging to no cluster.</param>
        /// <param name="rg">The input data object for saving the state.</param>
        private void SaveClustersForPlot(ICollection<DataFrame> outlierClusters, DataFrame outliers, RequestGroup rg) {
            if (outlierClusters.Count <= 0)
                return;

            rg.AnomalyDetectionMergedClusters = outliers.Clone();
            foreach (var cluster in outlierClusters)
                rg.AnomalyDetectionMergedClusters.Append(cluster.Rows, true);
        }

        /// <summary>
        /// Checks if a cluster is formed by distribution's natural outliers. If not, it smooths it using Modified Z-Score.
        /// Finally, it checks if the cluster is big enough for an anomaly and if it's not in tolerance.
        /// </summary>
        /// <param name="outlierClusters">Clusters of outliers.</param>
        /// <param name="validData">Valid data for reference.</param>
        /// <param name="rg">The input data object for reference and saving the anomalies.</param>
        /// <param name="anomalyLimit">Fraction of total amount of input data specifying minimum anomaly size.</param>
        /// <param name="tolerance">Defines how far from valid data in milliseconds a potential anomaly is not considered as an anomaly.</param>
        private void DecideForAnomalies(IEnumerable<DataFrame> outlierClusters, DataFrame validData, RequestGroup rg, double anomalyLimit, double tolerance) {
            var minAnomalySize = Math.Max(3, Math.Round(anomalyLimit * rg.TotalCount));
            foreach (var cluster in outlierClusters) {
                if (_clusterer.ClusterIsFromNaturalOutliers(cluster, validData))
                    continue;

                var finalCluster = SmoothCluster(cluster, rg.Outliers);
                var clusterDurations = finalCluster.Get<double>(Constants.Duration);
                var validDurations = validData.Get<double>(Constants.Duration);
                var inTolerance = Math.Abs(clusterDurations.Minimum() - validDurations.Maximum()) < tolerance ||
                                  Math.Abs(clusterDurations.Maximum() - validDurations.Minimum()) < tolerance;

                if (finalCluster.Length() >= minAnomalySize && !inTolerance)
                    rg.Anomalies.Append(finalCluster.Rows, true);
            }
        }

        /// <summary>
        /// A cluster might contain noise points around itself which it shouldn't or, on the other hand,
        /// might not contain those it should. It smooths (cuts or widens) the cluster to its final form.
        /// </summary>
        /// <param name="cluster">Cluster to be smoothed.</param>
        /// <param name="outliers">All outliers.</param>
        /// <returns>Smoothed cluster.</returns>
        private DataFrame SmoothCluster(DataFrame cluster, DataFrame outliers) {
            const int modificationSizeLimit = 15;
            const double borderZScoreOfPeak = 1.45;
            const double zScoreLimitForExtension = 2.95;

            if (!(cluster.Length() > modificationSizeLimit))
                return cluster;

            var (finalCluster, clusterOutliers) = RunModifiedZScore(cluster);
            if (clusterOutliers.Rows.Any())
                return finalCluster;

            finalCluster = finalCluster.OrderBy(Constants.Duration);
            var clusterZScores = finalCluster.GetAsList<double>(Constants.ZScore);
            var meanBorderZScore = (clusterZScores.First() + clusterZScores.Last()) / 2.0;
            if (!(meanBorderZScore > borderZScoreOfPeak))
                return finalCluster;

            var clusterDurations = finalCluster.GetAsList<double>(Constants.Duration);
            var median = clusterDurations.Median();
            var mad = Statistics.MedianAbsoluteDeviation(clusterDurations, median);
            (finalCluster, _) = RunModifiedZScore(outliers, median, mad, zScoreLimitForExtension);

            return finalCluster;
        }
    }
}