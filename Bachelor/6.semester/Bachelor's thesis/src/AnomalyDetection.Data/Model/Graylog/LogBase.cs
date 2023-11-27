// File: LogBase.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

using System;

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Graylog
{
    /// <summary>Base class for logs downloaded from Graylog.</summary>
    public abstract class LogBase
    {
        public string RequestHandler { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }
}