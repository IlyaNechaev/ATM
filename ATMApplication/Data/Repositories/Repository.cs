using ATMApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ATMApplication.Data
{
    public class Repository<TEntity>
        where TEntity : class
    {
        private DbContext Context { get; init; }

        public Repository(DbContext context) => Context = context;

        public async Task AddAsync(TEntity entity, bool forceSave = true)
        {
            try
            {
                await Context.Set<TEntity>().AddAsync(entity);
                if (forceSave)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }
        }

        public async Task AddRangeAsync(List<TEntity> entity, bool forceSave = true)
        {
            if (entity.Count == 0) return;
            try
            {
                await Context.Set<TEntity>().AddRangeAsync(entity);
                if (forceSave)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            IEnumerable<TEntity> entities;
            try
            {
                entities = await Context.Set<TEntity>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }

            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params string[] inclusions)
        {
            IEnumerable<TEntity> entities;
            try
            {
                var contextEntites = Context.Set<TEntity>().AsQueryable();
                foreach (var inclusion in inclusions)
                {
                    contextEntites = contextEntites.Include(inclusion);
                }
                entities = await contextEntites.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }

            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate)
        {
            IEnumerable<TEntity> entities;

            try
            {
                entities = await Task.FromResult(Context.Set<TEntity>().Where(predicate));
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }

            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate, params string[] inclusions)
        {
            IEnumerable<TEntity> entities;

            try
            {
                var contextEntites = Context.Set<TEntity>().AsQueryable();
                foreach (var inclusion in inclusions)
                {
                    contextEntites = contextEntites.Include(inclusion);
                }

                entities = await Task.FromResult(contextEntites.Where(predicate));
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }

            return entities;
        }

        public async Task<TEntity> GetSingleAsync(Func<TEntity, bool> predicate)
        {
            TEntity entity;

            try
            {
                var entities = await Task.FromResult(Context.Set<TEntity>().Where(predicate));
                entity = entities.Single();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }

            return entity;
        }

        public async Task<TEntity> GetSingleAsync(Func<TEntity, bool> predicate, params string[] inclusions)
        {
            TEntity entity;

            try
            {
                var contextEntites = Context.Set<TEntity>().AsQueryable();
                foreach (var inclusion in inclusions)
                {
                    contextEntites = contextEntites.Include(inclusion);
                }

                var entities = await Task.FromResult(contextEntites.Where(predicate));
                entity = entities.Single();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }

            return entity;
        }

        public async Task UpdateAsync(TEntity entity, bool forceSave = true)
        {
            try
            {
                Context.Set<TEntity>().Update(entity);

                if (forceSave)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }
        }

        public async Task DeleteAsync(TEntity entity, bool forceSave = true)
        {
            try
            {
                Context.Set<TEntity>().Remove(entity);

                if (forceSave)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool forceSave = true)
        {
            try
            {
                Context.Set<TEntity>().RemoveRange(entities);
                if (forceSave)
                {
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex);
            }
        }
    }

    public class RepositoryException : Exception
    {
        public string ExceptionType { get; init; }
        public string FullMessage { 
            get
            {
                return $"{ExceptionType}: {Message}";
            }
        }
        public RepositoryException(Exception exception) : base(exception.Message, exception)
        {
            ExceptionType = exception.GetType().Name;
        }

        public RepositoryException(string exceptionType, string message, Exception innerException) : base(message, innerException)
        {
            ExceptionType = exceptionType;
        }
    
    }
}
