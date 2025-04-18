using CesiZen_Backend.Dtos.CategoryDtos;

namespace CesiZen_Backend.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto command);
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task UpdateCategoryAsync(int id, UpdateCategoryDto command);
        Task DeleteCategoryAsync(int id);
    }
}
