using CesiZen_Backend.Dtos.MenuDtos;

namespace CesiZen_Backend.Services.MenuService
{
    public interface IMenuService
    {
        Task<MenuDto> CreateMenuAsync(CreateMenuDto command);
        Task<MenuDto?> GetMenuByIdAsync(int id);
        Task<IEnumerable<MenuDto>> GetAllMenusAsync();
        Task UpdateMenuAsync(int id, UpdateMenuDto command);
        Task DeleteMenuAsync(int id);
    }
}
