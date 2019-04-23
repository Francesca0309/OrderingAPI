using Microsoft.AspNetCore.Mvc;
using OrderCoreAPI.Controllers;
using OrderCoreAPI.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace OrderCoreAPI_Tests
{
    public class ProductsControllerTest : ControllerTestBase
    {
        private ProductsController _productController;

        public ProductsControllerTest()
        {
            _productController = new ProductsController(ProductRepositoryMock.Object);
        } 

        [Fact]
        public void GetAll_ReturnOk()
        {
            ProductRepositoryMock.Setup(repo => repo.Get()).Returns(GetProducts());

            var result = _productController.Get();

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var listOfItems = Assert.IsAssignableFrom<List<Product>>(okObjectResult.Value);
            Assert.Equal(ProductNo, listOfItems.Count);
        }

        [Fact]
        public void GetById_ReturnOk()
        {
            ProductRepositoryMock.Setup(repo => repo.Get(TestProduct.Id)).Returns(TestProduct);

            var result = _productController.Get(TestProduct.Id);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsAssignableFrom<Product>(okObjectResult.Value);
            Assert.Equal(TestProduct.Id, item.Id);
            Assert.Equal(TestProduct.Name, item.Name);
            Assert.Equal(TestProduct.Price, item.Price);
        }

        [Fact]
        public void GetById_ReturnNotFound()
        {
            ProductRepositoryMock.Setup(repo => repo.Get(TestProduct.Id)).Returns(TestProduct);

            var newGuid = Guid.NewGuid();
            var result = _productController.Get(newGuid);

            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        
    }
}
