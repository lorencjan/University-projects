using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using AutoMapper;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Mapping;
using MovieDatabase.DAL;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Factories;

namespace MovieDatabase.BL.Repositories
{
    public class RepositoryBase<TEntity, TListDto, TDto>
        where TEntity : EntityBase, new()
        where TListDto : DtoListBase
        where TDto : DtoBase
    {
        protected readonly IDbContextFactory DbContextFactory;
        protected readonly Mapper Mapper;
        protected readonly Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> DbSetIncludes;
        protected readonly Func<TEntity, IEnumerable<EntityBase>>[] TablesToBeSynchronized;
        protected readonly Func<TEntity, IEnumerable<MoviePerson>>[] JoinTablesToBeSynchronized;
        protected readonly Func<TEntity, string, Expression<Func<TEntity, bool>>> Filter;

        public RepositoryBase(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> dbSetIncludes,
                              Func<TEntity, IEnumerable<EntityBase>>[] tablesToBeSynchronized,
                              Func<TEntity, IEnumerable<MoviePerson>>[] joinTablesToBeSynchronized,
                              Func<TEntity, string, Expression<Func<TEntity, bool>>> filter,
                              IDbContextFactory factory)
                              
        {
            Mapper = DtoMapper.CreateMapper();
            DbSetIncludes = dbSetIncludes;
            TablesToBeSynchronized = tablesToBeSynchronized;
            JoinTablesToBeSynchronized = joinTablesToBeSynchronized;
            Filter = filter;
            DbContextFactory = factory;
        }

        public TDto GetById(Guid id)
        {
            var entity = GetEntityById(id);
            return Mapper.Map<TEntity, TDto>(entity);
        }

        private TEntity GetEntityById(Guid id)
        {
            using (var dbContext = DbContextFactory.CreateDbContext())
            {
                IQueryable<TEntity> dbSet = dbContext.Set<TEntity>();
                if (DbSetIncludes != null)
                {
                    dbSet = DbSetIncludes(dbSet);
                }
                return dbSet.FirstOrDefault(x => x.Id == id);
            }
        }

        public List<TDto> GetAll(string filter = null)
        {
            using (var dbContext = DbContextFactory.CreateDbContext())
            {
                IQueryable<TEntity> dbSet = dbContext.Set<TEntity>();
                if (DbSetIncludes != null)
                {
                    dbSet = DbSetIncludes(dbSet);
                }
                return dbSet
                    .Where(Filter(new TEntity(), filter))
                    .Select(x => Mapper.Map<TEntity, TDto>(x))
                    .ToList();
            }
        }

        public List<TListDto> GetAllAsList(string filter = null)
        {
            using (var dbContext = DbContextFactory.CreateDbContext())
            {
                IQueryable<TEntity> dbSet = dbContext.Set<TEntity>();
                return dbSet
                    .Where(Filter(new TEntity(), filter))
                    .Select(x => Mapper.Map<TEntity, TListDto>(x))
                    .ToList();
            }
        }

        public TDto InsertOrUpdate(TDto entityDto)
        {
            using (var dbContext = DbContextFactory.CreateDbContext())
            {
                var entity = Mapper.Map<TDto, TEntity>(entityDto);
                var entityEntry = dbContext.Update(entity);
                SynchronizeCollections(dbContext, entity);
                dbContext.SaveChanges();
                return GetById(entityEntry.Entity.Id);
            }
        }

        public void Delete(Guid id)
        {
            using (var dbContext = DbContextFactory.CreateDbContext())
            {
                dbContext.Remove(dbContext.Find(typeof(TEntity), id));
                dbContext.SaveChanges();
            }
        }


        /// <summary>
        /// If we update a collection item in program, it wouldn't reflect in db, it doesn't remove/update.
        /// Therefore it is necessary to delete the removed/updated ones, so they can be recreated.
        /// In addition, though not removing/changing, 1:N adds new ones, but N:N doesn't
        /// </summary>
        private void SynchronizeCollections(MovieDatabaseDbContext dbContext, TEntity entity)
        {
            //it's ok if we're adding new item and not updating ... everything creates well ON CREATE
            var entityInDb = GetEntityById(entity.Id);
            if (entityInDb == null)
                return;

            SynchronizeEntityTables(dbContext, entity, entityInDb);
            SynchronizeJoinTables(dbContext, entity, entityInDb);
        }

        /// <summary>
        /// One to many adds new items, only thing needed is to remove the changed ones
        /// </summary>
        private void SynchronizeEntityTables(MovieDatabaseDbContext dbContext, TEntity entity, TEntity entityInDb)
        {
            if (TablesToBeSynchronized == null)
                return;
            
            foreach (var collection in TablesToBeSynchronized)
            {
                var updatedCollection = collection(entity).ToArray();
                var collectionInDb = collection(entityInDb);
                foreach (var item in collectionInDb)
                {
                    if (updatedCollection.FirstOrDefault(x => x.Equals(item)) == null)
                    {
                        dbContext.Remove(dbContext.Find(item.GetType(), item.Id));
                    }
                }
            }
        }

        /// <summary>
        /// Many to many doesn't add new items -> we need to add them manually after the deletion
        /// </summary>
        private void SynchronizeJoinTables(MovieDatabaseDbContext dbContext, TEntity entity, TEntity entityInDb)
        {
            if (JoinTablesToBeSynchronized == null)
                return;

            foreach (var collection in JoinTablesToBeSynchronized)
            {
                var updatedCollection = collection(entity).ToArray();
                var collectionInDb = collection(entityInDb);
                //removes from db those changed
                foreach (var item in collectionInDb)
                {
                    if (updatedCollection.FirstOrDefault(x => x.Equals(item)) == null)
                    {
                        dbContext.Remove(dbContext.Find(item.GetType(), item.PersonId, item.MovieId));
                    }
                }
                //adds to db the new ones
                foreach (var item in updatedCollection)
                {
                    if (collectionInDb.FirstOrDefault(x => x.Equals(item)) == null)
                    {
                        dbContext.Add(item);
                    }
                }
            }
        }
    }
}