using System;

namespace RockFests.DAL.Types
{
    public class DateTimeInterval
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateTimeInterval(){}

        public DateTimeInterval(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public bool OverlapsWith(DateTimeInterval interval) => Start < interval.End && interval.Start < End;
    }
}