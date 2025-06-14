using CesiZen_Backend.Dtos.MenuDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.MenuService
{
    public class MenuService : IMenuService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<MenuService> _logger;
        public MenuService(CesiZenDbContext dbContext, ILogger<MenuService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<MenuResponseDto> CreateMenuAsync(CreateMenuRequestDto command)
        {
            Menu? parentMenu = await _dbContext.Menus.FindAsync(command.ParentId);

            int hierarchyLevel = parentMenu?.HierarchyLevel + 1 ?? 0;

            Menu menu = Menu.Create(command.Title, hierarchyLevel, command.ParentId);

            await _dbContext.Menus.AddAsync(menu);
            await _dbContext.SaveChangesAsync();

            return MenuMapper.ToDto(menu);
        }

        public async Task<IEnumerable<MenuResponseDto>> GetAllMenusAsync()
        {
            return await _dbContext.Menus
                .AsNoTracking()
                .Select(m => MenuMapper.ToDto(m))
                .ToListAsync();
        }

        public async Task<MenuResponseDto?> GetMenuByIdAsync(int id)
        {
            Menu? menu = await _dbContext.Menus
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
                return null;

            return MenuMapper.ToDto(menu);
        }

        public async Task UpdateMenuAsync(int id, UpdateMenuRequestDto command)
        {
            Menu? menuToUpdate = await _dbContext.Menus.FindAsync(id);
            if (menuToUpdate is null)
                throw new ArgumentNullException($"Invalid Menu Id.");

            Menu? parentMenu = await _dbContext.Menus.FindAsync(command.ParentId);

            int hierarchyLevel = parentMenu?.HierarchyLevel + 1 ?? 0;
            menuToUpdate.Update(command.Title, hierarchyLevel, command.ParentId);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMenuAsync(int id)
        {
            Menu? menuToDelete = await _dbContext.Menus.FindAsync(id);
            if (menuToDelete != null)
            {
                _dbContext.Menus.Remove(menuToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
