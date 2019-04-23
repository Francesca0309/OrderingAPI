using OrderCoreAPI.Models;
using System;
using System.Collections.Generic;

namespace OrderCoreAPI.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        T Get(Guid id);
        IEnumerable<T> Get();
        T Add(T entity);
        bool Delete(Guid id);
        bool Update(T t);
    }
}
