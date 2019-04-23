using System;
using Xunit;
using Moq;
using OrderCoreAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using OrderCoreAPI.Models;

namespace OrderCoreAPI_Tests
{
    public class OrdersControllerTest : ControllerTestBase
    {
        private OrdersController _orderController;

        public OrdersControllerTest()
        {
            _orderController = new OrdersController(OrderRepositoryMock.Object, ProductRepositoryMock.Object);
        }

        [Fact]
        public void GetAll_ReturnOk()
        {
            OrderRepositoryMock.Setup(repo => repo.Get()).Returns(GetOrders());

            var result = _orderController.Get();

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var listOfItems = Assert.IsAssignableFrom<List<Order>>(okObjectResult.Value);
            Assert.Single(listOfItems);
        }

        [Fact]
        public void GetById_ReturnOk()
        {
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);

            var result = _orderController.Get(TestOrder.Id);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
            Assert.Equal(TestOrder.Id, item.Id);
            Assert.Equal(TestOrder.Total, item.Total);
            Assert.Equal(TestOrder.OrderItems.Keys.Count, item.OrderItems.Keys.Count);
        }

        [Fact]
        public void GetById_ReturnNotFound()
        {
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);

            var newGuid = Guid.NewGuid();
            var result = _orderController.Get(newGuid);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_OrderEmpty_ReturnsCreatedAtAction()
        {
            var newOrder = CreateNewOrder();
            OrderRepositoryMock.Setup(repo => repo.Add(It.IsAny<Order>())).Returns(newOrder);

            var result = _orderController.Post(null);

            var createdObjectResult = Assert.IsType<CreatedAtActionResult>(result);
            var item = Assert.IsAssignableFrom<Order>(createdObjectResult.Value);
            Assert.NotEqual(item.Id, Guid.Empty);
            Assert.Equal(0, item.Total);
            Assert.NotNull(item.OrderItems);
        }

        [Fact]
        public void Create_WhenAlreadyExistingOrder_ReturnConflict()
        {
            OrderRepositoryMock.Setup(repo => repo.Add(TestOrder)).Returns<Order>(null);

            var result = _orderController.Post(TestOrder);

            Assert.IsType<ConflictResult>(result);
        }

        [Fact]
        public void Update_OrderDoesNotExist_ReturnNotFound()
        {
            var newOrder = CreateNewOrder();
            OrderRepositoryMock.Setup(repo => repo.Get(newOrder.Id)).Returns<Order>(null);

            var result = _orderController.Put(newOrder.Id,null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_OrderExists_OrderParameterIsNotNull_UpdateOrder_ReturnOk()
        {
            var newOrder = CreateNewOrder();
            newOrder.Id = TestOrder.Id;
            OrderRepositoryMock.Setup(repo => repo.Get(newOrder.Id)).Returns(newOrder);
            OrderRepositoryMock.Setup(repo => repo.Update(newOrder)).Returns(true);

            var result = _orderController.Put(newOrder.Id, TestOrder);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
            Assert.Equal(TestOrder.Id, item.Id);
            Assert.Equal(TestOrder.Total, item.Total);
            Assert.NotNull(item.OrderItems);
            Assert.Equal(TestOrder.OrderItems.Count,item.OrderItems.Count);
        }

        [Fact]
        public void Update_OrderExists_OrderParameterIsNull_ClearOrderItems_ReturnOk()
        {
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);
            OrderRepositoryMock.Setup(repo => repo.ClearOrderItems(TestOrder.Id)).Returns(ClearTestOrderItems(TestOrder));

            var result = _orderController.Put(TestOrder.Id, null);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
            Assert.Equal(TestOrder.Id, item.Id);
            Assert.Equal(0, item.Total);
            Assert.NotNull(item.OrderItems);
            Assert.Empty(item.OrderItems);
        }

        [Fact]
        public void Update_OrderExists_InvalidParameters_ReturnBadRequest()
        {
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);

            var result = _orderController.Put(TestOrder.Id, Guid.Empty, 0);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Update_OrderExists_IncreaseQuantity_ReturnOk()
        {
            var increasedQuantity = 2;
            var updatedTestOrder = ChangeOrderItemQuantity(TestOrder, increasedQuantity);
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);
            OrderRepositoryMock.Setup(repo => repo.Update(TestOrder.Id,TestProduct,increasedQuantity)).Returns(updatedTestOrder);
            ProductRepositoryMock.Setup(repo => repo.Get(TestProduct.Id)).Returns(TestProduct);

            var result = _orderController.Put(TestOrder.Id, TestProduct.Id, increasedQuantity);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
            Assert.Equal(updatedTestOrder.Id, item.Id);
            Assert.Equal(updatedTestOrder.Total, item.Total);
            Assert.Equal(updatedTestOrder.OrderItems[TestProduct.Id].Quantity, item.OrderItems[TestProduct.Id].Quantity);
            Assert.Equal(updatedTestOrder.OrderItems[TestProduct.Id].SubTotal, item.OrderItems[TestProduct.Id].SubTotal);
        }

        [Fact]
        public void Update_OrderExists_DecreaseQuantity_ReturnOk()
        {
            var decreasedQuantity = -1;
            var updatedTestOrder = ChangeOrderItemQuantity(TestOrder, decreasedQuantity);
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);
            OrderRepositoryMock.Setup(repo => repo.Update(TestOrder.Id, TestProduct, decreasedQuantity)).Returns(updatedTestOrder);
            ProductRepositoryMock.Setup(repo => repo.Get(TestProduct.Id)).Returns(TestProduct);

            var result = _orderController.Put(TestOrder.Id, TestProduct.Id, decreasedQuantity);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
            Assert.Equal(updatedTestOrder.Id, item.Id);
            Assert.Equal(updatedTestOrder.Total, item.Total);
            Assert.Equal(updatedTestOrder.OrderItems[TestProduct.Id].Quantity, item.OrderItems[TestProduct.Id].Quantity);
            Assert.Equal(updatedTestOrder.OrderItems[TestProduct.Id].SubTotal, item.OrderItems[TestProduct.Id].SubTotal);
        }

        [Fact]
        public void Update_OrderExists_RemoveOrderItem_ReturnOk()
        {
            var decreasedQuantity = -2;
            var updatedTestOrder = ChangeOrderItemQuantity(TestOrder, decreasedQuantity);
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);
            OrderRepositoryMock.Setup(repo => repo.Update(TestOrder.Id, TestProduct, decreasedQuantity)).Returns(updatedTestOrder);
            ProductRepositoryMock.Setup(repo => repo.Get(TestProduct.Id)).Returns(TestProduct);

            var result = _orderController.Put(TestOrder.Id, TestProduct.Id, decreasedQuantity);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
            Assert.Equal(updatedTestOrder.Id, item.Id);
            Assert.Equal(updatedTestOrder.Total, item.Total);
            Assert.DoesNotContain(TestProduct.Id, updatedTestOrder.OrderItems.Keys);
        }

        [Fact]
        public void Delete_WhenOrderAlreadyExists_ReturnOk()
        {
            OrderRepositoryMock.Setup(repo => repo.Get(TestOrder.Id)).Returns(TestOrder);
            OrderRepositoryMock.Setup(repo => repo.Delete(TestOrder.Id)).Returns(true);

            var result = _orderController.Delete(TestOrder.Id);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Delete_WhenOrderDoesNotExist_ReturnNotFound()
        {
            var newGuid = Guid.NewGuid();
            OrderRepositoryMock.Setup(repo => repo.Get(newGuid)).Returns<Order>(null);

            var result = _orderController.Delete(newGuid);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
