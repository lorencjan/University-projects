using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.BL.Model;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Factories;

namespace MovieDatabase.BL.Repositories
{
    public class MovieRepository : RepositoryBase<Movie, MovieListDto, MovieDto>
    {
        public MovieRepository(MovieDatabaseDbContextFactory factory)
            : base(movie => movie
                            .Include(m => m.Actors)
                            .ThenInclude(mact => mact.Person)
                            .Include(m => m.Directors)
                            .ThenInclude(mdir => mdir.Person)
                            .Include(m => m.Ratings),
                    new Func<Movie, IEnumerable<EntityBase>>[]
                    {
                    movie => movie.Ratings    
                    },
                    new Func<Movie, IEnumerable<MoviePerson>>[]
                    {
                        movie => movie.Actors,
                        movie => movie.Directors
                    },
                    (movie, filter) => m => string.IsNullOrEmpty(filter) ||
                                            m.OriginalName != null && m.OriginalName.ToLower().Contains(filter.ToLower())||
                                            m.CzechName != null && m.CzechName.ToLower().Contains(filter.ToLower())||
                                            m.Country != null && m.Country.ToLower().Contains(filter.ToLower())||
                                            m.Description != null && m.Description.ToLower().Contains(filter.ToLower()),
                    factory)
                                            
        {}
    }
}
