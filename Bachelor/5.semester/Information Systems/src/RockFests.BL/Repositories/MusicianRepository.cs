using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Attributes;
using RockFests.DAL.Entities;

namespace RockFests.BL.Repositories
{
    [RegisterService]
    public class MusicianRepository : RepositoryBase<Musician, MusicianDto>
    {
        public MusicianRepository(RockFestsDbContext dbContext)
            : base(musician => musician
                    .Include(b => b.Bands).ThenInclude(bm => bm.Band)
                    .Include(b => b.Performances).ThenInclude(p => p.Stage).ThenInclude(s => s.Festival)
                    .Include(b => b.Ratings),
                (musician, filter) => m => string.IsNullOrWhiteSpace(filter) || m.FirstName.ToLower().Contains(filter.ToLower()) || m.LastName.ToLower().Contains(filter.ToLower()) || m.Role.ToLower().Contains(filter.ToLower()),
                dbContext)
        { }

        public async Task<List<MusicianLightDto>> GetAllLight(string filter = null)
            => await DbContext.Musicians
                .Where(Filter(new Musician(), filter))
                .AsNoTracking()
                .Include(b => b.Ratings)
                .ProjectTo<MusicianLightDto>()
                .ToListAsync();
    }
}
