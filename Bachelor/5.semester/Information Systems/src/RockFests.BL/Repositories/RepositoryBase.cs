using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Entities;

namespace RockFests.BL.Repositories
{
    public class RepositoryBase<TEntity, TDto>
        where TEntity : BaseEntity, new()
        where TDto : BaseDto
    {
        protected readonly RockFestsDbContext DbContext;
        protected readonly Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> DbSetIncludes;
        protected readonly Func<TEntity, string, Expression<Func<TEntity, bool>>> Filter;

        public RepositoryBase(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> dbSetIncludes,
                              Func<TEntity, string, Expression<Func<TEntity, bool>>> filter,
                              RockFestsDbContext dbContext)
                              
        {
            DbSetIncludes = dbSetIncludes;
            Filter = filter;
            DbContext = dbContext;
        }

        public virtual async Task<TDto> GetById(int id)
        {
            IQueryable<TEntity> dbSet = DbContext.Set<TEntity>();
            if (DbSetIncludes != null)
            {
                dbSet = DbSetIncludes(dbSet);
            }
            var entity = await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return entity == null ? null : Mapper.Map<TDto>(entity);
        }

        public virtual async Task<List<TDto>> GetAll(string filter = null)
        {
            IQueryable<TEntity> dbSet = DbContext.Set<TEntity>();
            if (DbSetIncludes != null)
                dbSet = DbSetIncludes(dbSet);
            if (Filter != null)
                dbSet = dbSet.Where(Filter(new TEntity(), filter));
            //apparently ProjectTo cannot be used in this generic DbSet
            var entities = await dbSet.AsNoTracking().ToListAsync();
            return entities.AsQueryable().ProjectTo<TDto>().ToList();
        }

        public virtual async Task<int> Add(TDto entityDto)
        {
            var entity = Mapper.Map<TEntity>(entityDto);
            var created = await DbContext.Set<TEntity>().AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return created.Entity.Id;
        }

        public virtual async Task Update(TDto entityDto)
        {
            var entity = Mapper.Map<TEntity>(entityDto);
            DbContext.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task Delete(int id)
        {
            var toRemove = await DbContext.FindAsync(typeof(TEntity), id);
            if (toRemove == null)
                return;
            DbContext.Remove(toRemove);
            await DbContext.SaveChangesAsync();
        }
    }
}