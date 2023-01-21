using System.Linq;
using AutoMapper;
using MovieDatabase.BL.Model;
using MovieDatabase.DAL.Entities;

namespace MovieDatabase.BL.Mapping
{
    public static class DtoMapper
    {
        public static Mapper CreateMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                SetPersonListMapping(cfg);
                SetMovieListMapping(cfg);
                SetRatingListMapping(cfg);
                SetPersonMapping(cfg);
                SetMovieMapping(cfg);
                SetRatingMapping(cfg);
            });

            return new Mapper(config);
        }

        private static void SetPersonListMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Person, PersonListDto>()
                .ForMember(pDto => pDto.DisplayName, customMap => customMap.MapFrom(person => $"{person.FirstName} {person.LastName}"));
        }

        private static void SetMovieListMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Movie, MovieListDto>()
                .ForMember(mDto => mDto.DisplayName, customMap => customMap.MapFrom(movie => movie.OriginalName));
        }

        private static void SetRatingListMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Rating, RatingListDto>()
                .ForMember(mDto => mDto.DisplayName, customMap => customMap.MapFrom(r => $"{r.Movie.OriginalName} - {r.Number}/10"));
        }

        private static void SetPersonMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Person, PersonDto>()
                .ForMember(pDto => pDto.MoviesPlayedIn,
                           customMap => customMap.MapFrom(p => p.MoviesPlayedIn.Select(ma => CreateMovieListDtoFromMoviePerson(ma))))
                .ForMember(pDto => pDto.MoviesDirected,
                           customMap => customMap.MapFrom(p => p.MoviesDirected.Select(md => CreateMovieListDtoFromMoviePerson(md))));
            config.CreateMap<PersonDto, Person>()
                    .ForMember(pDb => pDb.MoviesDirected,
                               customMap => customMap.MapFrom(pDto => pDto.MoviesDirected.Select(mlDto => new MovieDirector
                               {
                                   MovieId = mlDto.Id,
                                   PersonId = pDto.Id
                               })))
                    .ForMember(pDb => pDb.MoviesPlayedIn,
                               customMap => customMap.MapFrom(pDto => pDto.MoviesPlayedIn.Select(mlDto => new MovieActor
                               {
                                   MovieId = mlDto.Id,
                                   PersonId = pDto.Id
                               })));
        }

        private static void SetMovieMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Movie, MovieDto>()
                .ForMember(mDto => mDto.Actors,
                           customMap => customMap.MapFrom(m => m.Actors.Select(ma => CreatePersonListDtoFromMoviePerson(ma))))
                .ForMember(mDto => mDto.Directors,
                           customMap => customMap.MapFrom(m => m.Directors.Select(md => CreatePersonListDtoFromMoviePerson(md))))
                .ForMember(mDto => mDto.Ratings,
                           customMap => customMap.MapFrom(m => m.Ratings.Select(r => new RatingDto
                           {
                               Id = r.Id,
                               Text = r.Text,
                               Number = r.Number,
                               MovieId = r.MovieId,
                               MovieName = r.Movie.OriginalName
                           })));
            config.CreateMap<MovieDto, Movie>()
                    .ForMember(mDb => mDb.Actors,
                               customMap => customMap.MapFrom(mDto => mDto.Actors.Select(plDto => new MovieActor
                               {
                                   MovieId = mDto.Id,
                                   PersonId = plDto.Id
                               })))
                    .ForMember(mDb => mDb.Directors,
                               customMap => customMap.MapFrom(mDto => mDto.Directors.Select(plDto => new MovieDirector
                               {
                                   MovieId = mDto.Id,
                                   PersonId = plDto.Id
                               })))
                    .ForMember(mDb => mDb.Ratings,
                               customMap => customMap.MapFrom(mDto => mDto.Ratings.Select(rDto => new Rating
                               {
                                   Id = rDto.Id,
                                   Text = rDto.Text,
                                   Number = rDto.Number,
                                   MovieId = mDto.Id
                               })));
        }

        private static void SetRatingMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Rating, RatingDto>()
                .ForMember(rDto => rDto.MovieName, customMap => customMap.MapFrom(rating => rating.Movie.OriginalName));
            config.CreateMap<RatingDto, Rating>();
        }

        private static MovieListDto CreateMovieListDtoFromMoviePerson(MoviePerson moviePerson)
        {
            return new MovieListDto
            {
                Id = moviePerson.MovieId,
                DisplayName = moviePerson.Movie.OriginalName
            };
        }
        
        private static PersonListDto CreatePersonListDtoFromMoviePerson(MoviePerson moviePerson)
        {
            return new PersonListDto
            {
                Id = moviePerson.PersonId,
                DisplayName = $"{moviePerson.Person.FirstName} {moviePerson.Person.LastName}"
            };
        }
    }
}
