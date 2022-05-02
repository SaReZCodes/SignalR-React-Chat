using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SRRC.DataLayer.Database;
using SRRC.Service.IRepository;

namespace SRRC.Service.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly SRRCDbContext Context;

        public Repository(SRRCDbContext context)
        {
            Context = context;
        }
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }
        public async Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
            return await Context.Set<TEntity>().AddAsync(entity);
        }
        public async void AddRangeAsync(IEnumerable<TEntity> entity)
        {
            await Context.Set<TEntity>().AddRangeAsync(entity);
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }
        public TEntity Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }
        public TEntity Get(long id)
        {
            return Context.Set<TEntity>().Find(id);
        }
        public TEntity Get(string id)
        {
            return Context.Set<TEntity>().Find(id);
        }
        public async Task<TEntity> GetAsync(long id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }
        public async Task<TEntity> GetAsync(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }
        public async Task<TEntity> GetAsync(string id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }
        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<TEntity> entity)
        {
            Context.Set<TEntity>().RemoveRange(entity);
        }
    }
}
