// File: RequestDataPointMap.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

using CsvHelper.Configuration;

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Csv
{
    /// <summary>Mapper class for CsvHelper to process <c>RequestDataPoint</c>.</summary>
    public sealed class RequestDataPointMap : ClassMap<RequestDataPoint>
    {
        public RequestDataPointMap() {
            Map(x => x.Timestamp).TypeConverterOption.Format("dd/MM/yyyy hh:mm:ss.fff tt");
            Map(x => x.Duration);
            Map(x => x.Errors);
        }
    }
}