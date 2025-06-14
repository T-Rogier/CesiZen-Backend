using CesiZen_Backend.Dtos.MenuDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.MenuService
{
    public class MenuMapper
    {
        public static MenuResponseDto ToDto(Menu menu)
        {
            return new MenuResponseDto(
                menu.Id,
                menu.Title,
                menu.HierarchyLevel,
                menu.ParentId
            );
        }
    }
}
