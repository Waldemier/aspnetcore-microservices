using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext: ICatalogContext
    {
        // DESCRIPTION: With Mongo database we do not need to create tables and migrations. 
        // The only thing we need is to create a collection only with mongo cli.
        public CatalogContext(IConfiguration configuration)
        {
            // For connection with mongo database
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
            CatalogContextSeed.SeedData(Products);
        }
        
        public IMongoCollection<Product> Products { get; }
    }
}