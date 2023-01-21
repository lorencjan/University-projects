using Microsoft.EntityFrameworkCore;
using MovieDatabase.BL.Model;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Factories;


namespace MovieDatabase.BL.Repositories
{
    public class RatingRepository : RepositoryBase<Rating, RatingListDto, RatingDto>
    {
        public RatingRepository(MovieDatabaseDbContextFactory factory) 
            : base(rating => rating.Include(r => r.Movie),
                   null,
                   null,
                   (rating, filter) => r => string.IsNullOrEmpty(filter) ||
                                            r.Text != null && r.Text.ToLower().Contains(filter.ToLower()),
                   factory)
        {}
    }
}
