using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OrderCoreAPI.Models;
using OrderCoreAPI.Repository;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductRepository _productRepository;
        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Get all the products
        /// </summary>
        /// <returns>A response containing a list of all the products in the system</returns>
        [HttpGet("")]
        [SwaggerResponse(200, "Status: OK - All the products present in the system.")]
        public IActionResult Get()
        {
            List<Product> products = _productRepository.Get().ToList();
            return Ok(products);
        }

        /// <summary>
        /// Get the product with the specified id
        /// </summary>
        /// <param name="id">The product id</param>
        /// <returns>A response containg the product object if found</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Status: OK - The product for the specified Id was returned.")]
        [SwaggerResponse(404, "Status: Not Found - The product for the specified Id does not exist.")]
        public IActionResult Get(Guid id)
        {
            Product product = _productRepository.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}