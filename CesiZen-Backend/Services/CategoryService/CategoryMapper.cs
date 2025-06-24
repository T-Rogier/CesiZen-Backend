using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.CategoryService
{
    public class CategoryMapper
    {
        public static CategoryResponseDto ToDto(Category category)
        {
            return new CategoryResponseDto(
                category.Id,
                category.Name,
                category.IconLink,
                category.Deleted
            );
        }

        public static CategoryListResponseDto ToListDto(List<Category> categories, int totalCount, int pageNumber = 1, int pageSize = 10)
        {
            return new CategoryListResponseDto(
                categories.Select(ToDto),
                pageNumber,
                pageSize,
                totalCount,
                totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0)
            );
        }
    }
}
