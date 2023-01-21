using System;
using MovieDatabase.BL.Model;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class MovieListDtoTests
    {
        private MovieListDto _first;
        private MovieListDto _second;

        public MovieListDtoTests()
        {
            _first = new MovieListDto()
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                DisplayName = "TEST"
            };
            _second = new MovieListDto()
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                DisplayName = "TEST"
            };
        }

        [Fact]
        public void MovieListDtoPass()
        {
            Assert.Equal(_first, _second);
        }
        [Fact]
        public void MovieListDtoFail()
        {
            _second.DisplayName = "Something else";
            Assert.NotEqual(_first, _second);
        }
    }
}
