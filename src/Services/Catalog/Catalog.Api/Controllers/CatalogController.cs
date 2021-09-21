using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(statusCode:StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode:(int)HttpStatusCode.OK, Type = typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productRepository.GetProducts();
            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(Product))]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productRepository.GetProductById(id: id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet]
        [Route("[action]/{categoryName}", Name = "GetProductByCategory")]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string categoryName)
        {
            var product = await _productRepository.GetProductByCategory(categoryName: categoryName);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(value: product);
        }

        [HttpPost]
        [ProducesResponseType(type: typeof(Product), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(Product))]
        public async Task<ActionResult<Product>> CreateProduct([FromBody]Product product)
        {
            await _productRepository.CreateProduct(product);
            return CreatedAtRoute(routeName: "GetProduct", routeValues: new {id = product.Id}, value: product);
        }

        [HttpPut]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(Product))]
        public async Task<IActionResult> UpdateProduct([FromBody]Product product)
        {
            return Ok(await _productRepository.UpdateProduct(product: product));
        }

        [HttpDelete(template: "{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(Product))]
        public async Task<ActionResult<bool>> DeleteProduct(string id)
        {
            return Ok(await _productRepository.DeleteProduct(id));
        }
    }
}