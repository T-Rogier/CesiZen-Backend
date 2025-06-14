using CesiZen_Backend.Dtos.MenuDtos;

namespace CesiZen_Backend.Services.MenuService
{
    public interface IMenuService
    {
        Task<MenuResponseDto> CreateMenuAsync(CreateMenuRequestDto command);
        Task<MenuResponseDto?> GetMenuByIdAsync(int id);
        Task<IEnumerable<MenuResponseDto>> GetAllMenusAsync();
        Task UpdateMenuAsync(int id, UpdateMenuRequestDto command);
        Task DeleteMenuAsync(int id);
    }
}
