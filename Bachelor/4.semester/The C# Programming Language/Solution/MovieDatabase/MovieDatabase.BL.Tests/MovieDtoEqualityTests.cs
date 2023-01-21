using System.Collections.Generic;
using MovieDatabase.DAL.Enums;
using MovieDatabase.DAL.Seeds;
using MovieDatabase.BL.Model;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class MovieDtoTests
    {
        private MovieDto _first;
        private MovieDto _second;

        public MovieDtoTests()
        {
            _first = new MovieDto()
            {
                OriginalName = "Test",
                CzechName = "zkouska",
                Genre = Genre.SciFi,
                TitlePhoto = SeedImages.Titanic,
                Country = "Testova republika",
                Year = 2020,
                Duration = 200,
                Description = "Testujeme"
            };
            _second = new MovieDto()
            {
                OriginalName = "Test",
                CzechName = "zkouska",
                Genre = Genre.SciFi,
                TitlePhoto = SeedImages.Titanic,
                Country = "Testova republika",
                Year = 2020,
                Duration = 200,
                Description = "Testujeme"
            };
        }
        
        [Fact]
        public void MovieDtoWithoutListsPass()
        {
            Assert.Equal(_first, _second);
        }
        [Fact]
        public void MovieDtoWithoutListsFail()
        {
            _second.Country = "Something else";
            Assert.NotEqual(_first, _second);
        }
        [Fact]
        public void MovieDtoWithListsPass()
        {
            var actors = new List<PersonListDto>()
            {
                new PersonListDto { DisplayName = "Test1"},
                new PersonListDto { DisplayName = "Test2"}
            };
            _first.Actors = actors;
            _second.Actors = actors;
            Assert.Equal(_first, _second);
        }
        [Fact]
        public void MovieDtoWithListsFail()
        {
            var actors1 = new List<PersonListDto>()
            {
                new PersonListDto { DisplayName = "Test1"},
                new PersonListDto { DisplayName = "Test2"}
            };
            var actors2 = new List<PersonListDto>()
            {
                new PersonListDto { DisplayName = "Test1"}
            };
            _first.Actors = actors1;
            _second.Actors = actors2;
            Assert.NotEqual(_first, _second);
        }
    }
}
