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

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto command)
        {
            var category = Category.Create(command.Name, command.IconLink);

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return CategoryMapper.ToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .Select(c => CategoryMapper.ToDto(c))
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _dbContext.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return null;

            return CategoryMapper.ToDto(category);
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto command)
        {
            var categoryToUpdate = await _dbContext.Categories.FindAsync(id);
            if (categoryToUpdate is null)
                throw new ArgumentNullException($"Invalid Category Id.");
            categoryToUpdate.Update(command.Name, command.IconLink);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var categoryToDelete = await _dbContext.Categories.FindAsync(id);
            if (categoryToDelete != null)
            {
                _dbContext.Categories.Remove(categoryToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
