using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    // Взаимодействие с контекстом баз данных
    public interface IDbService
    {
        public Task SetEntityProperty<EntityType, ValueType>(EntityType entity, string propertyName, ValueType propertyValue);

        public ValueType GetEntityProperty<EntityType, ValueType>(EntityType entity, string propertyName);
    }
}
