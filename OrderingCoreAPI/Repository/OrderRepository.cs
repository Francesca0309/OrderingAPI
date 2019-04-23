using OrderCoreAPI.Models;
using System;

namespace OrderCoreAPI.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public Order ClearOrderItems(Guid id)
        {
            var order = DataStub<Order>.Items[id];
            
            order.OrderItems.Clear();
            order.Total = 0;

            return order;
        }

        public Order Update(Guid orderId, Product product, int quantity)
        {
            var order = DataStub<Order>.Items[orderId];
            var orderItems = order.OrderItems;
            if (!orderItems.TryGetValue(product.Id, out OrderItem existingOrderItem))
            {
                if(quantity > 0)
                {
                    var orderItem = new OrderItem()
                    {
                        Product = product,
                        Quantity = quantity,
                        SubTotal = product.Price * quantity
                    };
                    orderItems.Add(product.Id, orderItem);
                    order.Total += orderItem.SubTotal;
                }    
            }
            else
            {
                var newQuantity = Math.Max(0,existingOrderItem.Quantity + quantity);
                if(newQuantity == 0)
                {
                    order.Total -= existingOrderItem.SubTotal;
                    orderItems.Remove(product.Id);
                }
                else
                {
                    var subTotalToAdd = quantity * product.Price;
                    existingOrderItem.Quantity = newQuantity;   
                    existingOrderItem.SubTotal += subTotalToAdd;
                    order.Total += subTotalToAdd;
                }   
            }
            return order;
        }
    }
}
