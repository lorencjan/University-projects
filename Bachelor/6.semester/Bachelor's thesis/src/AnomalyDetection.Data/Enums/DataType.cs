// File: DataType.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

namespace YSoft.Rqa.AnomalyDetection.Data.Enums
{
    /// <summary>
    /// Data is divided into those processed which are than used as reference data
    /// and those just downloaded but over which the detection hasn't been done yet.
    /// </summary>
    public enum DataType
    {
        Reference,
        Downloaded,
        All
    }
}