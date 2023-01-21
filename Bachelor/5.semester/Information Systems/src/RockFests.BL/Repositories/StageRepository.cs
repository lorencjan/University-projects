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
    public class StageRepository : RepositoryBase<Stage, StageDto>
    {
        public StageRepository(RockFestsDbContext dbContext)
            : base(stage => stage.Include(s => s.Festival).Include(s => s.Performances), null, dbContext)
        {}

        public async Task AddRange(IEnumerable<StageDto> stages)
        {
            var entities = stages.AsQueryable().ProjectTo<Stage>().ToList();
            await DbContext.Stages.AddRangeAsync(entities);
            await DbContext.SaveChangesAsync();
        }

        public override async Task Update(StageDto stageDto)
        {
            var performances = await DbContext.Performances.Where(x => x.StageId == stageDto.Id).AsNoTracking().ToListAsync();

            var stage = Mapper.Map<Stage>(stageDto);
            DbContext.Update(stage);

            //one to many adds new items, but doesn't remove
            var toRemove = performances.Where(p => stageDto.Performances.All(pDto => pDto.Id != p.Id));
            DbContext.Performances.RemoveRange(toRemove);

            await DbContext.SaveChangesAsync();
        }
    }
}
