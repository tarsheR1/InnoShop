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
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var product = await _productService.GetByIdAsync(query);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetUserProductsQuery(userId);
            var products = await _productService.GetUserProductsAsync(query);

            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchProductsQuery query)
        {
            var result = await _productService.SearchProductsAsync(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var product = await _productService.CreateProductAsync(command with { UserId = userId });
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var updatedCommand = command with { ProductId = id };
                var product = await _productService.UpdateProductAsync(updatedCommand, userId);

                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var command = new DeleteProductCommand(id, userId);
                var result = await _productService.DeleteProductAsync(command);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
