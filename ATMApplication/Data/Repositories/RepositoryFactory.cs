using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Data
{
    public class RepositoryFactory : IRepositoryFactory
    {
        DbContext Context { get; init; }

        public RepositoryFactory([FromServices] DbContext context)
        {
            Context = context;
        }

        public Repository<TEntity> GetRepository<TEntity>() where TEntity : class => new Repository<TEntity>(Context);
    }
}
