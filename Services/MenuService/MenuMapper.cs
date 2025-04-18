using CesiZen_Backend.Dtos.MenuDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.MenuService
{
    public class MenuMapper
    {
        public static MenuDto ToDto(Menu menu)
        {
            return new MenuDto(
                menu.Id,
                menu.Title,
                menu.HierarchyLevel,
                menu.ParentId
            );
        }
    }
}
