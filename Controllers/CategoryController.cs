using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Services.CategoryService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _CategoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto command)
        {
            var category = await _CategoryService.CreateCategoryAsync(command);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _CategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _CategoryService.GetCategoryByIdAsync(id);
            return category is null ? NotFound(new { Message = $"Category with ID {id} not found." }) : Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto command)
        {
            await _CategoryService.UpdateCategoryAsync(id, command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _CategoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
