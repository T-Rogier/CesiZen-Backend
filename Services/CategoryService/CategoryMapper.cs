using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.CategoryService
{
    public class CategoryMapper
    {
        public static CategoryDto ToDto(Category category)
        {
            return new CategoryDto(
                category.Id,
                category.Name,
                category.IconLink,
                category.Deleted
            );
        }
    }
}
