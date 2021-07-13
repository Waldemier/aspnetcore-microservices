using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly ICatalogContext _context;
        public ProductRepository(ICatalogContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetProductsAsync() =>
            await this._context.Products
                .Find(p => true)
                .ToListAsync();

        public async Task<Product> GetProductAsync(string id) => 
            await this._context.Products
                .Find(p => p.Id.Equals(id))
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetProductByNameAsync(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);

            return await this._context.Products
                .Find(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategoryAsync(string category)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, category);

            return await this._context.Products
                .Find(filter)
                .ToListAsync();
        }

        public async Task CreateProductAsync(Product product) => 
            await this._context.Products
                .InsertOneAsync(product);

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var updated =
                await this._context.Products
                    .ReplaceOneAsync(filter: p => p.Id.Equals(product.Id), replacement: product);

            return updated.IsAcknowledged 
                   && updated.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            DeleteResult result = await this._context.Products
                                            .DeleteOneAsync(filter);

            return result.IsAcknowledged 
                   && result.DeletedCount > 0;
        }
    }
}