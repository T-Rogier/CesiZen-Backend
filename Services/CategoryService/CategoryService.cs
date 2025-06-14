using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(CesiZenDbContext dbContext, ILogger<CategoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto command)
        {
            Category? category = Category.Create(command.Name, command.IconLink);

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return CategoryMapper.ToDto(category);
        }

        public async Task<CategoryListResponseDto> GetAllCategoriesAsync()
        {
            List<Category> categories = await _dbContext.Categories
                .AsNoTracking()
                .ToListAsync();
            return CategoryMapper.ToListDto(categories, 1, 10000, categories.Count);
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            Category? category = await _dbContext.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return null;

            return CategoryMapper.ToDto(category);
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryRequestDto command)
        {
            Category? categoryToUpdate = await _dbContext.Categories.FindAsync(id);
            if (categoryToUpdate is null)
                throw new ArgumentNullException($"Invalid Category Id.");
            categoryToUpdate.Update(command.Name, command.IconLink);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            Category? categoryToDelete = await _dbContext.Categories.FindAsync(id);
            if (categoryToDelete != null)
            {
                _dbContext.Categories.Remove(categoryToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
