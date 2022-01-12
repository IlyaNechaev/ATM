using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Data
{
    public interface IRepositoryFactory
    {
        public Repository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}
