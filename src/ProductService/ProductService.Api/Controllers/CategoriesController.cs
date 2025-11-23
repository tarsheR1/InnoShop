using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Interfaces;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var query = new GetAllCategoriesQuery();
            var categories = await _categoryService.GetAllCategoriesAsync(query);
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                throw new FileNotFoundException($"Category with id {id} not found");

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var category = await _categoryService.CreateCategoryAsync(command);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            var updatedCommand = command with { CategoryId = id };
            var category = await _categoryService.UpdateCategoryAsync(updatedCommand);

            if (category == null)
                throw new FileNotFoundException($"Category with id {id} not found");

            return Ok(category);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
                throw new FileNotFoundException($"Category with id {id} not found");

            return NoContent();
        }
    }
}