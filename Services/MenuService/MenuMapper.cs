using CesiZen_Backend.Dtos.MenuDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ArticleService;

namespace CesiZen_Backend.Services.MenuService
{
    public class MenuMapper
    {
        public static SimpleMenuResponseDto ToSimpleDto(Menu menu)
        {
            return new SimpleMenuResponseDto(
                menu.Id,
                menu.Title,
                menu.HierarchyLevel,
                menu.ParentId
            );
        }

        public static FullMenuResponseDto ToFullDto(Menu menu)
        {
            return new FullMenuResponseDto(
                menu.Id,
                menu.Title,
                menu.HierarchyLevel,
                menu.Children?.Select(ToFullDto).ToList(),
                menu.Articles?.Select(ArticleMapper.ToSimpleDto).ToList()
            );
        }
    }
}
