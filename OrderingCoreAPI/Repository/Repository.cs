using OrderCoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OrderCoreAPI.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : EntityBase
    {
        public T Get(Guid id)
        {
            return DataStub<T>.Items.TryGetValue(id, out T existingItem)
                ? existingItem
                : null;
        }

        public IEnumerable<T> Get()
        {
            return DataStub<T>.Items.Values;
        }

        public T Add(T entity)
        {
            if (!DataStub<T>.Items.Keys.Contains(entity.Id))
            {
                DataStub<T>.Items.Add(entity.Id, entity);
                return entity;
            }
            return null;
        }

        public bool Update(T entity)
        {
            if (DataStub<T>.Items.Keys.Contains(entity.Id))
            {
                DataStub<T>.Items[entity.Id] = entity;
                return true;
            }
            return false;
        }

        public bool Delete(Guid id)
        {
            return DataStub<T>.Items.Remove(id);
        }
    }
}
