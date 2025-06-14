using CesiZen_Backend.Dtos.CategoryDtos;

namespace CesiZen_Backend.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto command);
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryListResponseDto> GetAllCategoriesAsync();
        Task UpdateCategoryAsync(int id, UpdateCategoryRequestDto command);
        Task DeleteCategoryAsync(int id);
    }
}
