using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController: ControllerBase
    {
        // TODO: In future refactor responses status codes
        
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;
        
        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync() =>
            Ok(await this._repository.GetProductsAsync());

        [HttpGet("{id:length(24)}", Name = "GetProductAsync")]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductAsync(string id)
        {
            var product = await this._repository.GetProductAsync(id);
            
            if (product is null)
            {
                this._logger.LogError($"Product with {id} was not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet]
        [Route("[action]/{category}", Name = "GetProductByCategoryAsync")]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductByCategoryAsync(string category) =>
            Ok(await this._repository.GetProductByCategoryAsync(category));

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.Created)]
        public async Task<ActionResult<Product>> CreateProductAsync([FromBody] Product product)
        {
            await this._repository.CreateProductAsync(product);
            
            return CreatedAtRoute(nameof(GetProductAsync), new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] Product product) =>
            Ok(await this._repository.UpdateProductAsync(product));

        [HttpDelete("{id:length(24)}", Name = "DeleteProductAsync")]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductByIdAsync(string id) =>
            Ok(await this._repository.DeleteProductAsync(id));
    }
}