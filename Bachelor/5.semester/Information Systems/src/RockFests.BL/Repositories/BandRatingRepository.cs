using Microsoft.EntityFrameworkCore;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Attributes;
using RockFests.DAL.Entities;

namespace RockFests.BL.Repositories
{
    [RegisterService]
    public class BandRatingRepository : RepositoryBase<BandRating, RatingDto>
    {
        public BandRatingRepository(RockFestsDbContext dbContext)
            : base(ratings => ratings.Include(r => r.Band), null, dbContext)
        { }
    }
}
