using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Attributes;
using RockFests.DAL.Entities;

namespace RockFests.BL.Repositories
{
    [RegisterService]
    public class FestivalRepository : RepositoryBase<Festival, FestivalDto>
    {
        public FestivalRepository(RockFestsDbContext dbContext)
            : base(musician => musician
                    .Include(b => b.Stages).ThenInclude(s => s.Performances).ThenInclude(p => p.Musician)
                    .Include(b => b.Stages).ThenInclude(s => s.Performances).ThenInclude(p => p.Band)
                    .Include(b => b.Tickets),
                (festival, filter) => f => string.IsNullOrWhiteSpace(filter) || f.Name.ToLower().Contains(filter.ToLower()) || f.Location.ToLower().Contains(filter.ToLower()),
                dbContext)
        { }

        public async Task<List<FestivalLightDto>> GetAllLight(string filter = null, bool includePast = false)
        {
            var query = DbContext.Festivals.Where(Filter(new Festival(), filter));
            if (!includePast)
            {
                query = query.Where(x => x.StartDate > DateTime.Now);
            }
            return await query.OrderBy(x => x.StartDate).ProjectTo<FestivalLightDto>().ToListAsync();
        }

        public async Task<FestivalDto> GetClosestFestival()
        { 
            var festival = await DbContext.Festivals
                .OrderBy(x => x.StartDate)
                .Where(x => x.StartDate > DateTime.Now)
                .FirstOrDefaultAsync();
            return Mapper.Map<FestivalDto>(festival);
        }

        public async Task<FestivalDto> GetClosestUsersFestival(int userId)
        {
            var festival = await DbContext.Tickets
                .Where(x => x.UserId == userId)
                .Include(x => x.Festival)
                .Select(x => x.Festival)
                .OrderBy(x => x.StartDate)
                .Where(x => x.StartDate > DateTime.Now)
                .FirstOrDefaultAsync();
            return Mapper.Map<FestivalDto>(festival);
        }
    }
}
