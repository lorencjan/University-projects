using System;
using System.Collections.Generic;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Seeds;
using MovieDatabase.DAL.Enums;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Mapping;
using AutoMapper;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class PersonMapperTests
    {
        private Person _person;
        private PersonDto _personDto;
        private Mapper _mapper;

        public PersonMapperTests()
        {
            _person = new Person()
            {
                Id = new Guid("74fa3424-b2e8-470d-92eb-011b75290055"),
                FirstName = "Brad",
                LastName = "Pitt",
                Age = 56,
                Photo = SeedImages.BradPitt,
                Country = "USA",
                MoviesPlayedIn = new List<MovieActor>
                {
                    new MovieActor
                    {
                        PersonId = new Guid("74fa3424-b2e8-470d-92eb-011b75290055"),
                        MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                        Movie = new Movie
                        {
                            Id = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                            OriginalName = "Troy",
                            CzechName = "Trója",
                            Genre = Genre.Historical,
                            TitlePhoto = SeedImages.Troy,
                            Country = "USA",
                            Year = 2004,
                            Duration = 157,
                            Description = "Troy is a 2004 epic historical war drama film directed by Wolfgang Petersen and written by David Benioff. Produced by units in Malta, Mexico and Britain's Shepperton Studios, the film features an ensemble cast led by Brad Pitt, Eric Bana, and Orlando Bloom. It is loosely based on Homer's Iliad in its narration of the entire story of the decade-long Trojan War—condensed into little more than a couple of weeks, rather than just the quarrel between Achilles and Agamemnon in the ninth year. Achilles leads his Myrmidons along with the rest of the Greek army invading the historical city of Troy, defended by Hector's Trojan army. The end of the film (the sack of Troy) is not taken from the Iliad, but rather from Quintus Smyrnaeus's Posthomerica as the Iliad concludes with Hector's death and funeral. Troy made over $497 million worldwide, temporarily placing it in the #60 spot of top box office hits of all time. It received a nomination for the Academy Award for Best Costume Design and was the 8th highest-grossing film of 2004."
                        }
                    },
                    new MovieActor
                    {
                        PersonId = new Guid("74fa3424-b2e8-470d-92eb-011b75290055"),
                        MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                        Movie = new Movie
                        {
                            Id = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                            OriginalName = "Once Upon a Time in Hollywood",
                            CzechName = "Tenkrát v Hollywoodu",
                            Genre = Genre.Comedy,
                            TitlePhoto = SeedImages.OnceUponATimeInHollywood,
                            Country = "USA",
                            Year = 2019,
                            Duration = 161,
                            Description = "Once Upon a Time in Hollywood is a 2019 comedy-drama film written and directed by Quentin Tarantino. Produced by Columbia Pictures, Bona Film Group, Heyday Films, and Visiona Romantica and distributed by Sony Pictures Releasing, it is a co-production between the United States and the United Kingdom. It features a large ensemble cast led by Leonardo DiCaprio, Brad Pitt, and Margot Robbie. Set in 1969 Los Angeles, the film follows an actor and his stunt double, as they navigate the changing film industry, and features multiple storylines in a modern fairy tale tribute to the final moments of Hollywood's golden age."
                        }
                    }
                },
                MoviesDirected = new List<MovieDirector>()
            };
            foreach (var movie in _person.MoviesPlayedIn)
                movie.Person = _person;
            _personDto = new PersonDto()
            {
                Id = new Guid("74fa3424-b2e8-470d-92eb-011b75290055"),
                FirstName = "Brad",
                LastName = "Pitt",
                Age = 56,
                Photo = SeedImages.BradPitt,
                Country = "USA",
                MoviesPlayedIn = new List<MovieListDto>
                {
                    new MovieListDto { Id = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"), DisplayName = "Troy" },
                    new MovieListDto { Id = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"), DisplayName = "Once Upon a Time in Hollywood" }
                },
                MoviesDirected = new List<MovieListDto>()
            };
            _mapper = DtoMapper.CreateMapper();
        }

        [Fact]
        public void Person2PersonDtoWithoutLists()
        {
            _person.MoviesPlayedIn = new List<MovieActor>();
            _personDto.MoviesPlayedIn = new List<MovieListDto>();
            var personDto = _mapper.Map<Person, PersonDto>(_person);
            Assert.Equal(personDto, _personDto);
        }
        [Fact]
        public void PersonDto2PersonWithoutLists()
        {
            _person.MoviesPlayedIn = new List<MovieActor>();
            _personDto.MoviesPlayedIn = new List<MovieListDto>();
            var person = _mapper.Map<PersonDto, Person>(_personDto);
            Assert.Equal(person, _person);
        }
        [Fact]
        public void Person2PersonDtoPass()
        {
            var personDto = _mapper.Map<Person, PersonDto>(_person);
            Assert.Equal(personDto, _personDto);
        }
        [Fact]
        public void PersonDto2PersonPass()
        {
            var person = _mapper.Map<PersonDto, Person>(_personDto);
            Assert.Equal(person, _person);
        }
        [Fact]
        public void Person2PersonDtoFail()
        {
            _person.MoviesDirected.Add(new MovieDirector
            {
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                Movie = new Movie { Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"), OriginalName = "Titanic" },
                PersonId = _person.Id
            });
            var personDto = _mapper.Map<Person, PersonDto>(_person);
            Assert.NotEqual(personDto, _personDto);
        }
        [Fact]
        public void PersonDto2PersonFail()
        {
            _personDto.MoviesDirected.Add(new MovieListDto
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                DisplayName = "Titanic"
            });
            var person = _mapper.Map<PersonDto, Person>(_personDto);
            Assert.NotEqual(person, _person);
        }
        [Fact]
        public void Person2PersonListDtoInList()
        {
            var expectedPersonListDto = new PersonListDto()
            {
                Id = _person.Id,
                DisplayName = $"{_person.FirstName} {_person.LastName}"
            };
            var mappedPersonListDto = _mapper.Map<Person, PersonListDto>(_person);
            Assert.Equal(expectedPersonListDto, mappedPersonListDto);
        }
    }
}
