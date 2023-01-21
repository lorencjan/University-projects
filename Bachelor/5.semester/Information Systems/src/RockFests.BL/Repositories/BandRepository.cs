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
    public class BandRepository : RepositoryBase<Band, BandDto>
    {
        public BandRepository(RockFestsDbContext dbContext)
            : base(band => band
                    .Include(b => b.Members).ThenInclude(bm => bm.Musician)
                    .Include(b => b.Performances).ThenInclude(p => p.Stage).ThenInclude(s => s.Festival)
                    .Include(b => b.Ratings),
                (band, filter) => b => string.IsNullOrWhiteSpace(filter) || b.Name.ToLower().Contains(filter.ToLower()) || b.Genre.ToLower().Contains(filter.ToLower()),
                dbContext)
        { }

        public async Task<List<BandLightDto>> GetAllLight(string filter = null)
            => await DbContext.Bands
                .Where(Filter(new Band(), filter))
                .AsNoTracking()
                .Include(b => b.Ratings)
                .ProjectTo<BandLightDto>()
                .ToListAsync();

        public async Task AddMusician(int bandId, int musicianId)
            => await AddMusicians(bandId, new[] {musicianId});

        public async Task AddMusicians(int bandId, IEnumerable<int> musicianIds)
        {
            var bandMusicians = musicianIds.Select(id => new BandMusician {BandId = bandId, MusicianId = id});
            await DbContext.BandMusicians.AddRangeAsync(bandMusicians);
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteMusician(int bandId, int musicianId)
        {
            var bandMusician = await DbContext.BandMusicians.FindAsync(bandId, musicianId);
            if (bandMusician == null)
                return;
            DbContext.BandMusicians.Remove(bandMusician);
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteMusicians(int bandId, IEnumerable<int> musicianIds)
        {
            var bandMusicians = musicianIds.Select(id => new BandMusician { BandId = bandId, MusicianId = id });
            DbContext.BandMusicians.RemoveRange(bandMusicians);
            await DbContext.SaveChangesAsync();
        }
    }
}
