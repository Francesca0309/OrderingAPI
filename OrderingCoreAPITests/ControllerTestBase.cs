using Moq;
using OrderCoreAPI.Models;
using OrderCoreAPI.Repository;
using System;
using System.Collections.Generic;

namespace OrderCoreAPI_Tests
{
    public class ControllerTestBase
    {
        protected readonly int ProductNo = 10;
        protected Product TestProduct;
        protected Order TestOrder;
        
        public Mock<IOrderRepository> OrderRepositoryMock;
        public Mock<IProductRepository> ProductRepositoryMock;

        public ControllerTestBase()
        {
            CreateTestProduct();
            CreateTestOrder();

            ProductRepositoryMock = new Mock<IProductRepository>();
            OrderRepositoryMock = new Mock<IOrderRepository>();
        }

        private void CreateTestProduct()
        {
            TestProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product-Test",
                Price = 10
            };
        }

        private void CreateTestOrder()
        {
            var orderItems = new Dictionary<Guid, OrderItem>();
            orderItems.Add(TestProduct.Id, new OrderItem()
            {
                Product = TestProduct,
                Quantity = 2,
                SubTotal = 2 * TestProduct.Price
            });
            TestOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderItems = orderItems,
                Total = orderItems[TestProduct.Id].SubTotal
            };
        }

        protected Order CreateNewOrder()
        {
            return new Order()
            {
                Id = Guid.NewGuid(),
                OrderItems = new Dictionary<Guid, OrderItem>(),
                Total = 0
            };
        }

        protected List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            var randomNr = new Random();
            for (int i = 0; i < ProductNo; i++)
            {
                var guid = Guid.NewGuid();
                products.Add(new Product
                {
                    Id = guid,
                    Name = string.Format("Product {0}", i + 1),
                    Price = randomNr.Next(10, 50)
                });
            }
            return products;
        }

        protected List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();
            orders.Add(TestOrder);

            return orders;
        }

        protected Order ClearTestOrderItems(Order order)
        {
            order.OrderItems.Clear();
            order.Total = 0;

            return order;
        }

        protected Order ChangeOrderItemQuantity(Order order, int quantity)
        {
            var existingOrderItem = order.OrderItems[TestProduct.Id];
            var newQuantity = Math.Max(0, existingOrderItem.Quantity + quantity);
            if (newQuantity == 0)
            {
                order.Total -= existingOrderItem.SubTotal;
                order.OrderItems.Remove(TestProduct.Id);
            }
            else
            {
                var subTotalToAdd = quantity * TestProduct.Price;
                existingOrderItem.Quantity = newQuantity;
                existingOrderItem.SubTotal += subTotalToAdd;
                order.Total += subTotalToAdd;
            }

            return order;
        }
    }
}
