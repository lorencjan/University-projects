// File: RequestDataPoint.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

using System;

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Csv
{
    /// <summary>Single data point for a specific request of a service.</summary>
    public class RequestDataPoint
    {
        public double Duration { get; set; }
        public int Errors { get; set; }
        public DateTime Timestamp { get; set; }
    }
}