using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.BL.Model;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Factories;

namespace MovieDatabase.BL.Repositories
{
    public class PersonRepository : RepositoryBase<Person, PersonListDto, PersonDto>
    {
        public PersonRepository(MovieDatabaseDbContextFactory factory) 
            : base(person => person
                              .Include(p => p.MoviesPlayedIn)
                              .ThenInclude(mpi => mpi.Movie)
                              .Include(p => p.MoviesDirected)
                              .ThenInclude(mdir => mdir.Movie),
                    null,
                    new Func<Person, IEnumerable<MoviePerson>>[]
                    {
                    person => person.MoviesPlayedIn,
                    person => person.MoviesDirected
                    },
                    (person, filter) => p => string.IsNullOrEmpty(filter) ||
                                             p.FirstName != null && p.FirstName.ToLower().Contains(filter.ToLower())||
                                             p.LastName != null && p.LastName.ToLower().Contains(filter.ToLower()),
                    factory)
        {}
    }
}
