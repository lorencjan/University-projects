// File: RequestLog.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Graylog
{
    /// <summary>Class representing request logs downloaded from Graylog.</summary>
    public class RequestLog : LogBase
    {
        public double Duration { get; set; }
    }
}