using OrderCoreAPI.Models;
using System;
using System.Collections.Generic;

namespace OrderCoreAPI.Repository
{
    public class DataStub<T> where T : EntityBase
    {
        public static Dictionary<Guid, T> Items { get; set; } = new Dictionary<Guid, T>();
    }
}
