using System;
using System.Collections.Generic;

namespace OrderCoreAPI.Models
{
    public class Order : EntityBase
    {
        public Dictionary<Guid, OrderItem> OrderItems { get; set; }
        public decimal Total { get; set; }
    }
}
