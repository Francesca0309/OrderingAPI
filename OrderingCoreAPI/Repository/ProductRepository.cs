using OrderCoreAPI.Models;
using System;

namespace OrderCoreAPI.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly int productNo = 5;
        public ProductRepository()
        {
            Initialise();
        }

        public void Initialise()
        {
            var randomNr = new Random();
            for (int i = 0; i < productNo; i++)
            {
                var guid = Guid.NewGuid();
                DataStub<Product>.Items.Add(guid,
                    new Product
                    {
                        Id = guid,
                        Name = string.Format("Product {0}", i + 1),
                        Price = randomNr.Next(10, 50)
                    });
            }
        }
    }
}
