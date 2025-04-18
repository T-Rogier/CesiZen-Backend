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

        public async Task<MenuDto> CreateMenuAsync(CreateMenuDto command)
        {
            var parentMenu = await _dbContext.Menus.FindAsync(command.ParentId);

            int hierarchyLevel = parentMenu?.HierarchyLevel + 1 ?? 0;

            var menu = Menu.Create(command.Title, hierarchyLevel, command.ParentId);

            await _dbContext.Menus.AddAsync(menu);
            await _dbContext.SaveChangesAsync();

            return MenuMapper.ToDto(menu);
        }

        public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
        {
            return await _dbContext.Menus
                .AsNoTracking()
                .Select(m => MenuMapper.ToDto(m))
                .ToListAsync();
        }

        public async Task<MenuDto?> GetMenuByIdAsync(int id)
        {
            var menu = await _dbContext.Menus
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
                return null;

            return MenuMapper.ToDto(menu);
        }

        public async Task UpdateMenuAsync(int id, UpdateMenuDto command)
        {
            var menuToUpdate = await _dbContext.Menus.FindAsync(id);
            if (menuToUpdate is null)
                throw new ArgumentNullException($"Invalid Menu Id.");

            var parentMenu = await _dbContext.Menus.FindAsync(command.ParentId);

            int hierarchyLevel = parentMenu?.HierarchyLevel + 1 ?? 0;
            menuToUpdate.Update(command.Title, hierarchyLevel, command.ParentId);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMenuAsync(int id)
        {
            var menuToDelete = await _dbContext.Menus.FindAsync(id);
            if (menuToDelete != null)
            {
                _dbContext.Menus.Remove(menuToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
