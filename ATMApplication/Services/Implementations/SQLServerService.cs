using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public class SQLServerService : IDbService
    {
        DbContext Context;
        public SQLServerService([FromServices] DbContext context)
        {
            Context = context;
        }

        public async Task SetEntityProperty<EntityType, ValueType>(EntityType entity, string propertyName, ValueType propertyValue)
        {
            try
            {
                Context.Entry(entity).Property(propertyName).CurrentValue = propertyValue;
                await Context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public ValueType GetEntityProperty<EntityType, ValueType>(EntityType entity, string propertyName)
        {
            try
            {
                ValueType result = (ValueType)Context.Entry(entity).Property(propertyName).CurrentValue;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
