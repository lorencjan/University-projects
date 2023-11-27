// File: Interval.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

namespace YSoft.Rqa.AnomalyDetection.Application.Model
{
    /// <summary>Basic interval class for type double.</summary>
    public class Interval
    {
        public double Left { get; set; }
        public double Length => Right - Left;
        public double Right { get; set; }

        public Interval() { }

        public Interval(double left, double right) {
            Left = left;
            Right = right;
        }

        /// <summary>Checks if two intervals overlap each other.</summary>
        /// <param name="interval">Second interval to compare with.</param>
        /// <returns>True if the intervals overlap, false otherwise.</returns>
        public bool OverlapsWith(Interval interval) => Left < interval.Right && interval.Left < Right;
    }
}