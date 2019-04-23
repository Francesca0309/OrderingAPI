using OrderCoreAPI.Models;
using OrderingCoreAPIClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderingCoreAPIClientSample
{
    public class Program
    {
        public static string baseEndpoint = "https://localhost:44381/";

        public static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static void ShowProducts(List<Product> products)
        {
            foreach (var product in products)
            {
                Console.WriteLine($"Name: {product.Name}\tId: " +
                $"{product.Id}\tPrice: {product.Price}");
            }
        }

        static void ShowOrderItems(Order order)           
        {
            if(order != null && order.Id != Guid.Empty)
            {
                Console.WriteLine($"Order Id: {order.Id}\tTotal: " +
                                $"{order.Total}\t");
                foreach (var orderItem in order.OrderItems)
                {
                    Console.WriteLine($"Product Id: {orderItem.Key}\tProduct Name: " +
                    $"{orderItem.Value.Product.Name}\tQuantity: {orderItem.Value.Quantity}" +
                    $"\tSubtotal: {orderItem.Value.SubTotal}");
                }
            }  
        }

        static void Delimiter()
        {
            Console.WriteLine($"------------------------------------------");
        }

        static async Task RunAsync()
        {
            Guid orderId = new Guid(), 
                productId1, productId2;

            var apiClient = new ApiClient(baseEndpoint);

            try
            {
                Console.WriteLine($"Getting the existing products...");
                var products = await apiClient.GetProducts();
                ShowProducts(products);
                productId1 = products[0].Id;
                productId2 = products[1].Id;
                Delimiter();

                Console.WriteLine($"Creating an order...");
                var order = await apiClient.CreateOrder(new Order());
                if (order != null) {
                    orderId = order.Id;
                }
                Console.WriteLine($"Order created - Order Id: {orderId}");
                Delimiter();

                Console.WriteLine($"Adding 2 x 'Product 1' and 3 x 'Product 2' to the order...");
                var updatedOrder = await apiClient.UpdateOrder(orderId, productId1, 2);
                updatedOrder = await apiClient.UpdateOrder(orderId, productId2, 3);

                Console.WriteLine($"Getting the updated order details...");
                ShowOrderItems(updatedOrder);
                Delimiter();

                Console.WriteLine($"Decrease the quantity for 'Product 2' with 1...");
                updatedOrder = await apiClient.UpdateOrder(orderId, productId2, -1);

                Console.WriteLine($"Getting the updated order details...");
                ShowOrderItems(updatedOrder);
                Delimiter();

                Console.WriteLine($"Increase the quantity for 'Product 1' with 1...");
                updatedOrder = await apiClient.UpdateOrder(orderId, productId1, 1);

                Console.WriteLine($"Getting the updated order details...");
                ShowOrderItems(updatedOrder);
                Delimiter();

                Console.WriteLine($"Remove 'Product 1' from the order ...");
                updatedOrder = await apiClient.UpdateOrder(orderId, productId1, -3);

                Console.WriteLine($"Getting the updated order details...");
                ShowOrderItems(updatedOrder);
                Delimiter();

                Console.WriteLine($"Removing all the items from the order...");
                updatedOrder = await apiClient.UpdateOrder(orderId, new Order());

                Console.WriteLine($"Getting the updated order details...");
                ShowOrderItems(updatedOrder);
                Delimiter();

                Console.WriteLine($"Deleting the order...");
                var statusCode = await apiClient.DeleteOrder(orderId);
                Console.WriteLine($"Response: {(int)statusCode}");

                Console.WriteLine($"Attempting to delete the order again...");
                statusCode = await apiClient.DeleteOrder(orderId);
                Console.WriteLine($"Response: {(int)statusCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
