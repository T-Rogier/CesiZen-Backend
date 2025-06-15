using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Filters;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto command)
        {
            CategoryResponseDto category = await _CategoryService.CreateCategoryAsync(command);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            CategoryListResponseDto categories = await _CategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetCategoriesByFilter([FromQuery] CategoryFilterRequestDto filter)
        {
            CategoryListResponseDto activities = await _CategoryService.GetCategoriesByFilterAsync(filter);
            return Ok(activities);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            CategoryResponseDto? category = await _CategoryService.GetCategoryByIdAsync(id);
            return category is null ? NotFound(new { Message = $"Category with ID {id} not found." }) : Ok(category);
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto command)
        {
            await _CategoryService.UpdateCategoryAsync(id, command);
            return NoContent();
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _CategoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
