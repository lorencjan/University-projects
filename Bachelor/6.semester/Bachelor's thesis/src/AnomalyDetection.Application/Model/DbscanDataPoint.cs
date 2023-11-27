// File: DbscanDataPoint.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using DBSCAN;

namespace YSoft.Rqa.AnomalyDetection.Application.Model
{
    /// <summary>Customized data point for DBSCAN algorithm.</summary>
    public class DbscanPointData : IPointData
    {
        public int Index { get; }
        public int Label { get; set; } = -1;
        private readonly Point _point;

        public DbscanPointData(double x, int index) {
            _point = new Point(x, 0);
            Index = index;
        }

        public ref readonly Point Point => ref _point;
    }
}