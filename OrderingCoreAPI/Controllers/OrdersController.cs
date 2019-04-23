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
    public class OrdersController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Get all the orders
        /// </summary>
        /// <returns>A response containg a list of all the orders</returns>
        [HttpGet("")]
        [SwaggerResponse(200, "Status: OK - All the orders present in the system.")]
        public IActionResult Get()
        {
            List<Order> orders = _orderRepository.Get().ToList();
            return Ok(orders);
        }

        /// <summary>
        /// Get the order by the specified id
        /// </summary>
        /// <param name="id">The order id</param>
        /// <returns>A response containg the order object</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Status: OK - The order for the specified Id was returned.")]
        [SwaggerResponse(404, "Status: Not Found - The order for the specified Id does not exist.")]
        public IActionResult Get(Guid id)
        {
            Order order = _orderRepository.Get(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        /// <summary>
        /// Create an order
        /// </summary>
        /// <param name="order">If the parameter is null or empty, then an empty order is 
        /// created with a new generated id. Otherwise creates the order,with the details specified</param>
        /// <returns>A response containing the order object in case of a successful result</returns>
        [HttpPost]
        [SwaggerResponse(201, "Status: Created - The order was created.")]
        [SwaggerResponse(400, "Status: Bad Request - The model specified is not valid")]
        [SwaggerResponse(409, "Status: Conflict - The order id specified in the model already exists")]
        public IActionResult Post([FromBody]Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (order == null || order.Id == Guid.Empty)
            {
                order = new Order()
                {
                    Id = Guid.NewGuid(),
                    OrderItems = new Dictionary<Guid, OrderItem>(),
                    Total = 0
                };
            }

            Order result = _orderRepository.Add(order);

            if (result != null)
            {
                return CreatedAtAction(nameof(Get), new
                {
                    id = order.Id
                }, order);
            }
            else
            {
                return Conflict();
            }
        }

        /// <summary>
        /// Fully update the order if the order object is specified, otherwise clear up the
        /// order contents
        /// </summary>
        /// <param name="orderId">The order Id.</param>
        /// <param name="order">The order object. If it is not specified, then the order contents
        /// will be removed</param>
        /// <returns>A response containing the updated object in case of a successful result</returns>
        [HttpPut("{orderId}")]
        [SwaggerResponse(200, "Status: OK - The order for the specified Id was updated.")]
        [SwaggerResponse(404, "Status: Not Found - The order for the specified Id does not exist.")]
        public IActionResult Put(Guid orderId, [FromBody]Order order)
        {
            var existingOrder = _orderRepository.Get(orderId);
            if (existingOrder == null)
                return NotFound();

            if (order != null && order.Id != Guid.Empty)
            {
                _orderRepository.Update(order);
                return Ok(order);
            }
            else
            {
                var updatedOrder = _orderRepository.ClearOrderItems(orderId);
                return Ok(updatedOrder);
            }
        }

        /// <summary>
        /// Update the order by adding/removing products or increasing/decreasing 
        /// the quantity of existing order items
        /// </summary>
        /// <param name="orderId">The order Id</param>
        /// <param name="productId">The product id. If the order does not contain already an
        /// order item with this id, then it will be added to the order </param>
        /// <param name="quantity">The new quantity of the product. 
        /// This can be a positive or a negative number to increase or 
        /// decrease the quantity of the order item </param>
        /// <returns>A response containing also the updated object in case of a successful result</returns>
        [HttpPut("{orderId}/{productId}/{quantity}")]
        [SwaggerResponse(200, "Status: OK - The order for the specified Id was updated.")]
        [SwaggerResponse(404, "Status: Not Found - The order or the product for the specified Ids do not exist.")]
        [SwaggerResponse(400, "Status: Bad Request - The productId and quantity parameters need to be valid")]
        public IActionResult Put(Guid orderId, Guid productId, int quantity)
        {
            var existingOrder = _orderRepository.Get(orderId);
            if (existingOrder == null)
                return NotFound();

            if (productId == Guid.Empty && quantity == 0)
                return BadRequest();

            var existingProduct = _productRepository.Get(productId);
            if (existingProduct == null)
                return NotFound();

            var order = _orderRepository.Update(orderId, existingProduct, quantity);
            return Ok(order);
        }

        /// <summary>
        /// Delete an order and all its contents
        /// </summary>
        /// <param name="id">The order id</param>
        /// <returns>Return a response containing the appropriate status code</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Status: OK - The order for the specified Id was deleted.")]
        [SwaggerResponse(404, "Status: Not Found - The order for the specified Id does not exist.")]
        public IActionResult Delete(Guid id)
        {
            if (_orderRepository.Get(id) != null)
            {
                _orderRepository.Delete(id);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}