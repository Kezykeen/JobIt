using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobIT.web.Respositories.Generic
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> AddAsync(TEntity entity);
        ICollection<TEntity> ToList();
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IQueryable<TEntity>> Get(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetById(object Id);
        Task<TEntity> Update(TEntity entity);
        Task<bool> Delete(object Id);
        Task<int> Save();
    }
}
