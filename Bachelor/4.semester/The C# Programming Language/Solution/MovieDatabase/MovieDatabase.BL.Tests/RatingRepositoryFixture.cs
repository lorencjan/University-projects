using MovieDatabase.BL.Repositories;
using MovieDatabase.BL.Model;
using MovieDatabase.DAL.Factories;
using MovieDatabase.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MovieDatabase.BL.Tests
{
    public class RatingRepositoryFixture : RepositoryBase<Rating, RatingListDto, RatingDto>
    {
        public RatingRepositoryFixture(MovieDatabaseInMemoryDbContextFactory factory)
            : base(rating => rating.Include(r => r.Movie),
                   null,
                   null,
                   (rating, filter) => r => string.IsNullOrEmpty(filter) ||
                                            r.Text != null && r.Text.ToLower().Contains(filter.ToLower()),
                   factory)
        {}
    }
}
