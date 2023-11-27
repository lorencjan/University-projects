// File: DateTimeInterval.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

using System;

namespace YSoft.Rqa.AnomalyDetection.Data.Model.Graylog
{
    /// <summary>Interval of type DateTime with added functionality for Graylog's download window.</summary>
    public class DateTimeInterval
    {
        public DateTime End { get; set; }
        public TimeSpan Length => End - Start;
        public DateTime Start { get; set; }
        private readonly int? _limit;

        public DateTimeInterval() { }

        public DateTimeInterval(DateTime start, DateTime end, int? limit = null) {
            Start = start;
            End = end;
            _limit = limit;
        }

        /// <summary>Graylog download window is going to be limited for faster filtering on the Graylog's side.</summary>
        /// <param name="limit">Maximum number of days. Defaults to 1.</param>
        /// <returns>Interval with given boundaries but with <paramref name="limit" /> days as maximum.</returns>
        public DateTimeInterval LimitedInterval(int limit = 1) => Length.Days >= limit ? new DateTimeInterval(Start, Start.AddDays(limit), limit) : new DateTimeInterval(Start, End, limit);

        /// <summary>Updates download window of a limited interval.</summary>
        /// <param name="lastLog">Last log of current batch.</param>
        /// <param name="end">Limit to the download window.</param>
        public void MoveLimitWindow(LogBase lastLog, DateTime end) {
            if (!_limit.HasValue)
                return;

            Start = lastLog?.Timestamp.AddMilliseconds(1) ?? End;
            var movedEnd = Start.AddDays(_limit.Value);
            End = movedEnd > end ? end : movedEnd;
        }
    }
}