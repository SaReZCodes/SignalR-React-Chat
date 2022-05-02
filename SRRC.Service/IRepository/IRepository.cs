using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SRRC.Service.IRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        TEntity Get(long id);
        TEntity Get(string id);
        IEnumerable<TEntity> GetAll();
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity);
        void AddRangeAsync(IEnumerable<TEntity> entity);
        Task<TEntity> GetAsync(int id);
        Task<TEntity> GetAsync(string id);
        Task<List<TEntity>> GetAllAsync();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entity);
    }
}
