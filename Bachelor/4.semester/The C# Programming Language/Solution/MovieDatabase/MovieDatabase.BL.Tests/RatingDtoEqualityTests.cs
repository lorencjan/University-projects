using System;
using MovieDatabase.BL.Model;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class RatingDtoTests
    {
        private RatingDto _first;
        private RatingDto _second;

        public RatingDtoTests()
        {
            _first = new RatingDto()
            {
                Text = "Test",
                Number = 5,
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                MovieName = "TEST"
            };
            _second = new RatingDto()
            {
                Text = "Test",
                Number = 5,
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                MovieName = "TEST"
            };
        }

        [Fact]
        public void RatingDtoPass()
        {
            Assert.Equal(_first, _second);
        }
        [Fact]
        public void RatingDtoFail()
        {
            _second.MovieName = "Something else";
            Assert.NotEqual(_first, _second);
        }
    }
}
