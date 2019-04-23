using OrderCoreAPI.Models;
using System;

namespace OrderCoreAPI.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Order ClearOrderItems(Guid id);
        Order Update(Guid orderId, Product product, int quantity); 
    }
}
