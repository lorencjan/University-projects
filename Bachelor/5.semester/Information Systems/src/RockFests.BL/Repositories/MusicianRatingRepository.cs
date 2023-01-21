using Microsoft.EntityFrameworkCore;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Attributes;
using RockFests.DAL.Entities;

namespace RockFests.BL.Repositories
{
    [RegisterService]
    public class MusicianRatingRepository : RepositoryBase<MusicianRating, RatingDto>
    {
        public MusicianRatingRepository(RockFestsDbContext dbContext)
            : base(ratings => ratings.Include(r => r.Musician), null, dbContext)
        {}
    }
}
