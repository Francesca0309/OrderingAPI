using OrderCoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderingCoreAPIClient
{
    public class ApiClient : ApiClientBase
    {
        private readonly string _ordersApiPath = "api/orders";
        private readonly string _productsApiPath = "api/products";

        public ApiClient(string baseEndpoint) : base(baseEndpoint) {}

        //Products
        public async Task<List<Product>> GetProducts()
        {
            return await GetAllAsync<Product>(_productsApiPath);
        }

        public async Task<Product> GetProductById(Guid id)
        {
            return await GetAsync<Product>(_productsApiPath, id);
        }

        //Orders
        public async Task<Order> CreateOrder(Order order)
        {
            return await CreateAsync(_ordersApiPath, order);
        }

        public async Task<List<Order>> GetOrders()
        {
            return await GetAllAsync<Order>(_ordersApiPath);
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            return await GetAsync<Order>(_ordersApiPath, id);
        }

        public async Task<Order> UpdateOrder(Guid id, Order order)
        {
            return await UpdateAsync<Order>(_ordersApiPath, id, order);
        }

        public async Task<Order> UpdateOrder(Guid orderId, Guid productId, int quantity)
        {
            HttpResponseMessage response = await _httpClient.PutAsync(
               $"{BaseEndpoint}{_ordersApiPath}/{orderId}/{productId}/{quantity}", new StringContent(""));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Order>();
        }

        public async Task<HttpStatusCode> DeleteOrder(Guid id)
        {
            return await DeleteAsync(_ordersApiPath,id);
        }
    }
}
