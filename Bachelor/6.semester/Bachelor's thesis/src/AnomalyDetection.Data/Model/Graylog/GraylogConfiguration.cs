// File: GraylogConfiguration.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Graylog
{
    /// <summary>Configuration class for appsettings.json "GraylogConfiguration" section.</summary>
    public class GraylogConfiguration
    {
        public string ErrorLogsStreamId { get; set; }
        public string Host { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string RequestLogsStreamId { get; set; }
    }
}