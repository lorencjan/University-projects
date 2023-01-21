using System;
using System.Collections.Generic;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Enums;
using MovieDatabase.DAL.Seeds;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Mapping;
using AutoMapper;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class MovieMapperTests
    {
        private Movie _movie;
        private MovieDto _movieDto;
        private Mapper _mapper;

        public MovieMapperTests()
        {
            _movie = new Movie
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                OriginalName = "Titanic",
                CzechName = "Titanic",
                Genre = Genre.Catasthrophic,
                TitlePhoto = SeedImages.Titanic,
                Country = "USA",
                Year = 1997,
                Duration = 194,
                Description = "Titanic is a 1997 American epic romance and disaster film directed, written, co-produced, and co-edited by James Cameron. Incorporating both historical and fictionalized aspects, the film is based on accounts of the sinking of the RMS Titanic, and stars Leonardo DiCaprio and Kate Winslet as members of different social classes who fall in love aboard the ship during its ill-fated maiden voyage. Cameron's inspiration for the film came from his fascination with shipwrecks; he felt a love story interspersed with the human loss would be essential to convey the emotional impact of the disaster. Production began in 1995, when Cameron shot footage of the actual Titanic wreck. The modern scenes on the research vessel were shot on board the Akademik Mstislav Keldysh, which Cameron had used as a base when filming the wreck. Scale models, computer-generated imagery, and a reconstruction of the Titanic built at Baja Studios were used to re-create the sinking. The film was co-financed by Paramount Pictures and 20th Century Fox; the former handled distribution in North America while the latter released the film internationally. It was the most expensive film ever made at the time, with a production budget of $200 million.",
                Actors = new List<MovieActor>
                {
                    new MovieActor
                    { 
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                        PersonId = new Guid("f9008976-566d-40ab-b2bc-137bab9fb64d"),
                        Person = new Person
                        {
                            Id = new Guid("f9008976-566d-40ab-b2bc-137bab9fb64d"),
                            FirstName = "Kate",
                            LastName = "Winslet",
                            Age = 44,
                            Photo = SeedImages.KateWinslet,
                            Country = "Great Britain"
                        }
                    },
                    new MovieActor
                    { 
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                        PersonId = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5"),
                        Person = new Person
                        {
                            Id = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5"),
                            FirstName = "Leonardo",
                            LastName = "DiCaprio",
                            Age = 45,
                            Photo = SeedImages.LeonardoDiCaprio,
                            Country = "USA"
                        }
                    }
                },
                Directors = new List<MovieDirector>
                {
                    new MovieDirector
                    { 
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                        PersonId = new Guid("9f64adab-623b-4125-8e03-f64a4bc7edb0"),
                        Person = new Person
                        {
                            Id = new Guid("9f64adab-623b-4125-8e03-f64a4bc7edb0"),
                            FirstName = "James",
                            LastName = "Cameron",
                            Age = 65,
                            Photo = SeedImages.JamesCameron,
                            Country = "Canada"
                        }
                    }
                },
                Ratings = new List<Rating>
                {
                    new Rating
                    {
                        Id = new Guid("8ecf6824-ab94-4e39-b81d-223afd4cbe50"),
                        Text = "You can watch this movie in 1997, you can watch it again in 2004 or 2009 or you can watch it in 2015 or 2020, and this movie will get you EVERY TIME. Titanic has made itself FOREVER a timeless classic! I just saw it today (2015) and I was crying my eyeballs out JUST like the first time I saw it back in 1998. This is a movie that is SO touching, SO precise in the making of the boat, the acting and the storyline is BRILLIANT! And the preciseness of the ship makes it even more outstanding! Kate Winslet and Leonardo Dicaprio definitely created a timeless classic that can be watched time and time again and will never get old. This movie will always continue to be a beautiful, painful & tragic movie. 10/10 stars for this masterpiece!",
                        Number = 10,
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5")
                    },
                    new Rating
                    {
                        Id = new Guid("2068787a-4352-47b7-9453-85ed8c8664c3"),
                        Text = "To all the miserable people who have done everything from complain about the dialogue, the budget, the this and the that....who wants to hear it? IF you missed the point of this beyond-beautiful movie, that's your loss. The rest of us who deeply love this movie do not care what you think. I am a thirthysomething guy who has seen thousands of movies in my life, and this one stands in its own entity, in my book. It was not supposed to be a documentary, or a completely factual account of what happened that night. It is the most amazing love story ever attempted. I know that it is the cynical 90's and the millennium has everyone in a tizzy, but come on. Someone on this comments board complained that it made too much money! How lame is that? It made bundles of money in every civilized country on the planet, and is the top grossing film in the planet. I will gladly side with the majority this time around. Okay, cynics, time to crawl back under your rock, I am done.",
                        Number = 10,
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5")
                    }
                }
            };
            foreach (var actor in _movie.Actors)
                actor.Movie = _movie;
            foreach (var director in _movie.Directors)
                director.Movie = _movie;
            foreach (var rating in _movie.Ratings)
                rating.Movie = _movie;
            _movieDto = new MovieDto()
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                OriginalName = "Titanic",
                CzechName = "Titanic",
                Genre = Genre.Catasthrophic,
                TitlePhoto = SeedImages.Titanic,
                Country = "USA",
                Year = 1997,
                Duration = 194,
                Description = "Titanic is a 1997 American epic romance and disaster film directed, written, co-produced, and co-edited by James Cameron. Incorporating both historical and fictionalized aspects, the film is based on accounts of the sinking of the RMS Titanic, and stars Leonardo DiCaprio and Kate Winslet as members of different social classes who fall in love aboard the ship during its ill-fated maiden voyage. Cameron's inspiration for the film came from his fascination with shipwrecks; he felt a love story interspersed with the human loss would be essential to convey the emotional impact of the disaster. Production began in 1995, when Cameron shot footage of the actual Titanic wreck. The modern scenes on the research vessel were shot on board the Akademik Mstislav Keldysh, which Cameron had used as a base when filming the wreck. Scale models, computer-generated imagery, and a reconstruction of the Titanic built at Baja Studios were used to re-create the sinking. The film was co-financed by Paramount Pictures and 20th Century Fox; the former handled distribution in North America while the latter released the film internationally. It was the most expensive film ever made at the time, with a production budget of $200 million.",
                Actors = new List<PersonListDto>
                {
                    new PersonListDto { Id = new Guid("f9008976-566d-40ab-b2bc-137bab9fb64d"), DisplayName = "Kate Winslet" },
                    new PersonListDto { Id = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5"), DisplayName = "Leonardo DiCaprio" }
                },
                Directors = new List<PersonListDto> { new PersonListDto { Id = new Guid("9f64adab-623b-4125-8e03-f64a4bc7edb0"), DisplayName = "James Cameron" } },
                Ratings = new List<RatingDto>
                {
                    new RatingDto
                    {
                        Id = new Guid("8ecf6824-ab94-4e39-b81d-223afd4cbe50"),
                        Text = "You can watch this movie in 1997, you can watch it again in 2004 or 2009 or you can watch it in 2015 or 2020, and this movie will get you EVERY TIME. Titanic has made itself FOREVER a timeless classic! I just saw it today (2015) and I was crying my eyeballs out JUST like the first time I saw it back in 1998. This is a movie that is SO touching, SO precise in the making of the boat, the acting and the storyline is BRILLIANT! And the preciseness of the ship makes it even more outstanding! Kate Winslet and Leonardo Dicaprio definitely created a timeless classic that can be watched time and time again and will never get old. This movie will always continue to be a beautiful, painful & tragic movie. 10/10 stars for this masterpiece!",
                        Number = 10,
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                        MovieName = "Titanic"
                    },
                    new RatingDto
                    {
                        Id = new Guid("2068787a-4352-47b7-9453-85ed8c8664c3"),
                        Text = "To all the miserable people who have done everything from complain about the dialogue, the budget, the this and the that....who wants to hear it? IF you missed the point of this beyond-beautiful movie, that's your loss. The rest of us who deeply love this movie do not care what you think. I am a thirthysomething guy who has seen thousands of movies in my life, and this one stands in its own entity, in my book. It was not supposed to be a documentary, or a completely factual account of what happened that night. It is the most amazing love story ever attempted. I know that it is the cynical 90's and the millennium has everyone in a tizzy, but come on. Someone on this comments board complained that it made too much money! How lame is that? It made bundles of money in every civilized country on the planet, and is the top grossing film in the planet. I will gladly side with the majority this time around. Okay, cynics, time to crawl back under your rock, I am done.",
                        Number = 10,
                        MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                        MovieName = "Titanic"
                    }
                }
            };
            _mapper = DtoMapper.CreateMapper();
        }

        [Fact]
        public void Movie2MovieDtoWithoutLists()
        {
            _movie.Actors = new List<MovieActor>();
            _movie.Directors = new List<MovieDirector>();
            _movie.Ratings = new List<Rating>();
            _movieDto.Actors = new List<PersonListDto>();
            _movieDto.Directors = new List<PersonListDto>();
            _movieDto.Ratings = new List<RatingDto>();
            var movieDto = _mapper.Map<Movie, MovieDto>(_movie);
            Assert.Equal(movieDto, _movieDto);
        }
        [Fact]
        public void MovieDto2MovieWithoutLists()
        {
            _movie.Actors = new List<MovieActor>(); ;
            _movie.Directors = new List<MovieDirector>(); ;
            _movie.Ratings = new List<Rating>(); ;
            _movieDto.Actors = new List<PersonListDto>(); ;
            _movieDto.Directors = new List<PersonListDto>(); ;
            _movieDto.Ratings = new List<RatingDto>(); ;
            var movie = _mapper.Map<MovieDto, Movie>(_movieDto);
            Assert.Equal(movie, _movie);
        }
        [Fact]
        public void Movie2MovieDtoPass()
        {
            var movieDto = _mapper.Map<Movie, MovieDto>(_movie);
            Assert.Equal(movieDto, _movieDto);
        }
        [Fact]
        public void MovieDto2MoviePass()
        {
            var movie = _mapper.Map<MovieDto, Movie>(_movieDto);
            Assert.Equal(movie, _movie);
        }
        [Fact]
        public void Movie2MovieDtoFail()
        {
            _movie.Ratings = new List<Rating>();
            var movieDto = _mapper.Map<Movie, MovieDto>(_movie);
            Assert.NotEqual(movieDto, _movieDto);
        }
        [Fact]
        public void MovieDto2MovieFail()
        {
            _movieDto.Actors.RemoveAt(0);
            var movie = _mapper.Map<MovieDto, Movie>(_movieDto);
            Assert.NotEqual(movie, _movie);
        }
        [Fact]
        public void Movie2MovieListDtoInList()
        {
            var expectedMovieListDto = new MovieListDto()
            {
                Id = _movie.Id,
                DisplayName = _movie.OriginalName
            };
            var mappedMovieListDto = _mapper.Map<Movie, MovieListDto>(_movie);
            Assert.Equal(expectedMovieListDto, mappedMovieListDto);
        }
    }
}
