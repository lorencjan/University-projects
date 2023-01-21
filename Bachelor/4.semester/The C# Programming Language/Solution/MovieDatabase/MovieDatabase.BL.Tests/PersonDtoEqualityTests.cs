using System.Collections.Generic;
using MovieDatabase.DAL.Seeds;
using MovieDatabase.BL.Model;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class PersonDtoTests
    {
        private PersonDto _first;
        private PersonDto _second;

        public PersonDtoTests()
        {
            _first = new PersonDto()
            {
                FirstName = "Test",
                LastName = "Testovej",
                Age = 20,
                Photo = SeedImages.BradPitt,
                Country = "Testova republika"
            };
            _second = new PersonDto()
            {
                FirstName = "Test",
                LastName = "Testovej",
                Age = 20,
                Photo = SeedImages.BradPitt,
                Country = "Testova republika"
            };
        }

        [Fact]
        public void PersonDtoWithoutListsPass()
        {
            Assert.Equal(_first, _second);
        }
        [Fact]
        public void PersonDtoWithoutListsFail()
        {
            _second.Country = "Something else";
            Assert.NotEqual(_first, _second);
        }
        [Fact]
        public void PersonDtoWithListsPass()
        {
            var movies = new List<MovieListDto>()
            {
                new MovieListDto { DisplayName = "Test1"},
                new MovieListDto { DisplayName = "Test2"}
            };
            _first.MoviesPlayedIn = movies;
            _second.MoviesPlayedIn = movies;
            Assert.Equal(_first, _second);
        }
        [Fact]
        public void PersonDtoWithListsFail()
        {
            var actors1 = new List<MovieListDto>()
            {
                new MovieListDto { DisplayName = "Test1"},
                new MovieListDto { DisplayName = "Test2"}
            };
            var actors2 = new List<MovieListDto>()
            {
                new MovieListDto { DisplayName = "Test1"}
            };
            _first.MoviesPlayedIn = actors1;
            _second.MoviesPlayedIn = actors2;
            Assert.NotEqual(_first, _second);
        }
    }
}
