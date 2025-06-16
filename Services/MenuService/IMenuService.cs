using CesiZen_Backend.Dtos.MenuDtos;

namespace CesiZen_Backend.Services.MenuService
{
    public interface IMenuService
    {
        Task<SimpleMenuResponseDto> CreateMenuAsync(CreateMenuRequestDto command);
        Task<SimpleMenuResponseDto?> GetMenuByIdAsync(int id);
        Task<IEnumerable<SimpleMenuResponseDto>> GetAllMenusAsync();
        Task<IEnumerable<FullMenuResponseDto>> GetMenuHierarchyAsync();
        Task UpdateMenuAsync(int id, UpdateMenuRequestDto command);
        Task DeleteMenuAsync(int id);
    }
}
