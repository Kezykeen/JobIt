using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace JobIT.web.Respositories.Generic
{
    public class GenericRepository<TEntity> : IDisposable, IGenericRepository<TEntity> where TEntity : class
    {
        internal DbContext context;
        internal DbSet<TEntity> dbSet;
        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public async Task<bool> AddAsync(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ICollection<TEntity> ToList()
        {
            return dbSet.ToList();
        }

        public async Task<IQueryable<TEntity>> Get(Expression<Func<TEntity, bool>> predicate)
        {
            var query = dbSet.AsQueryable();
            if (predicate != null)
                return await Task.FromResult(query.Where(predicate));
            else
                return query;
        }

        public async Task<TEntity> GetById(object Id)
        {
            return await dbSet.FindAsync(Id);
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.SingleOrDefaultAsync(predicate);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return await Task.FromResult(entity);
        }
        public virtual bool Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            return true;
        }
        public async Task<bool> Delete(object Id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(Id);
            return Delete(entityToDelete);
        }

        public async Task<int> Save()
        {
            return await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}