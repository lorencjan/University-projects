using System;
using NUnit.Framework;
using RockFests.DAL.Types;

namespace RockFests.Specification.TypeTests
{
    public class DateTimeIntervalTests : TestFixture
    {
        public static DateTimeInterval GetHourInterval(int start, int end)
            => new DateTimeInterval(DateTime.Today.AddHours(start), DateTime.Today.AddHours(end));

        [TestCase(1,3,2,4)]
        [TestCase(2,4,1,3)]
        [TestCase(1,4,2,3)]
        [TestCase(2,3,1,4)]
        public void Overlapping_intervals_are_successfully_detected(int start1, int end1, int start2, int end2)
        {
            var first = GetHourInterval(start1, end1);
            var second = GetHourInterval(start2, end2);

            Assert.That(first.OverlapsWith(second));
        }

        [TestCase(1,2,4,5)]
        [TestCase(1,2,2,3)]
        [TestCase(2,3,1,2)]
        public void Intervals_that_does_not_overlap_are_not_detected(int start1, int end1, int start2, int end2)
        {
            var first = GetHourInterval(start1, end1);
            var second = GetHourInterval(start2, end2);

            Assert.That(!first.OverlapsWith(second));
        }
    }
}
