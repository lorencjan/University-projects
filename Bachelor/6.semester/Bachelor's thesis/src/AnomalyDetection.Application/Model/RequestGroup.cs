// File: RequestGroup.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Analysis;
using YSoft.Rqa.AnomalyDetection.Data.Model.Csv;

namespace YSoft.Rqa.AnomalyDetection.Application.Model
{
    /// <summary>Contains data of a specific service request type.</summary>
    public class RequestGroup
    {
        /// <summary>Outliers forming a cluster big enough to be considered an anomaly.</summary>
        public DataFrame Anomalies { get; set; }

        /// <summary>Initial (h)dbscan clusters in anomaly detection. Saved middle step for plotting purposes.</summary>
        public DataFrame AnomalyDetectionClusters { get; set; }

        /// <summary>Merged initial (h)dbscan clusters in anomaly detection. Saved middle step for plotting purposes.</summary>
        public DataFrame AnomalyDetectionMergedClusters { get; set; }

        /// <summary>Initial dbscan clusters in outlier detection. Saved middle step for plotting purposes.</summary>
        public DataFrame OutlierDetectionClusters { get; set; }

        /// <summary>Requests which are considered as outliers.</summary>
        public DataFrame Outliers { get; set; }

        /// <summary>Name of the request handler this works with.</summary>
        public string RequestHandler { get; }

        /// <summary>Name of the service this works with.</summary>
        public string Service { get; }

        /// <summary>Total amount of requests.</summary>
        public long TotalCount { get; }

        /// <summary>Requests which are considered as valid.</summary>
        public DataFrame ValidData { get; set; }

        public RequestGroup(string service, string handler, ICollection<RequestDataPoint> requests) {
            Service = service;
            RequestHandler = handler;

            var timestamps = new PrimitiveDataFrameColumn<DateTime>(Constants.Timestamp, requests.Select(x => x.Timestamp));
            var durations = new PrimitiveDataFrameColumn<double>(Constants.Duration, requests.Select(x => x.Duration));
            var errors = new PrimitiveDataFrameColumn<int>(Constants.Errors, requests.Select(x => x.Errors));
            var labels = new PrimitiveDataFrameColumn<int>(Constants.Label, requests.Count);
            var zScores = new PrimitiveDataFrameColumn<int>(Constants.ZScore, requests.Count);

            ValidData = new DataFrame(timestamps, durations, errors, labels, zScores);
            Outliers = EmptyDataSet();
            Anomalies = EmptyDataSet();
            TotalCount = ValidData.Length();
        }

        private DataFrame EmptyDataSet() => ValidData.Length() > 0 ? ValidData.Head(0) : ValidData.Clone();
    }
}