using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Interfaces;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var product = await _productService.GetByIdAsync(query);

            if (product == null)
                throw new FileNotFoundException($"Product with id {id} not found");

            return Ok(product);
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUserProducts()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            var query = new GetUserProductsQuery(userId);
            var products = await _productService.GetUserProductsAsync(query);

            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductsQuery query)
        {
            var result = await _productService.SearchProductsAsync(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            var product = await _productService.CreateProductAsync(command with { UserId = userId });
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            var updatedCommand = command with { ProductId = id };
            var product = await _productService.UpdateProductAsync(updatedCommand, userId);

            if (product == null)
                throw new FileNotFoundException($"Product with id {id} not found");

            return Ok(product);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            var command = new DeleteProductCommand(id, userId);
            var result = await _productService.DeleteProductAsync(command);

            if (!result)
                throw new FileNotFoundException($"Product with id {id} not found");

            return NoContent();
        }

        [HttpPost("users/{userId:string}/deactivate-products")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUserProducts(
        string userId,
        CancellationToken cancellationToken)
        {
            await _productService.DeactivateUserProductsAsync(userId);
            return Ok();
        }
    }
}