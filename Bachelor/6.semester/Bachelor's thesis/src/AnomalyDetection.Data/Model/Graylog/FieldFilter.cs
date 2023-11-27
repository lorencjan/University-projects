// File: FieldFilter.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Graylog
{
    /// <summary>Serves to specify a wanted value of a certain field in Graylog's filtering.</summary>
    public class FieldFilter
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
}