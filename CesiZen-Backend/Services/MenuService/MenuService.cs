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

        public async Task<SimpleMenuResponseDto> CreateMenuAsync(CreateMenuRequestDto command)
        {
            Menu? parentMenu = await _dbContext.Menus.FindAsync(command.ParentId);

            int hierarchyLevel = parentMenu?.HierarchyLevel + 1 ?? 0;

            Menu menu = Menu.Create(command.Title, hierarchyLevel, command.ParentId);

            await _dbContext.Menus.AddAsync(menu);
            await _dbContext.SaveChangesAsync();

            return MenuMapper.ToSimpleDto(menu);
        }

        public async Task<IEnumerable<SimpleMenuResponseDto>> GetAllMenusAsync()
        {
            return await _dbContext.Menus
                .AsNoTracking()
                .Select(m => MenuMapper.ToSimpleDto(m))
                .ToListAsync();
        }

        public async Task<IEnumerable<FullMenuResponseDto>> GetMenuHierarchyAsync()
        {
            List<Menu> menus = await _dbContext.Menus
                .AsNoTracking()
                .Where(m => m.ParentId == null)
                .ToListAsync();

            if (menus == null || menus.Count == 0)
                throw new ArgumentNullException($"Aucun menu trouvé");

            foreach (Menu menu in menus)
            {
                await LoadChildrenRecursiveAsync(menu);
            }

            Console.WriteLine(menus);

            return menus.Select(MenuMapper.ToFullDto);
        }

        private async Task LoadChildrenRecursiveAsync(Menu menu)
        {
            await _dbContext.Entry(menu)
                .Collection(m => m.Articles)
                .LoadAsync();

            await _dbContext.Entry(menu)
                .Collection(m => m.Children)
                .LoadAsync();

            foreach (Menu child in menu.Children)
            {
                await _dbContext.Entry(child)
                    .Collection(c => c.Articles)
                    .LoadAsync();

                await LoadChildrenRecursiveAsync(child);
            }
        }

        public async Task<SimpleMenuResponseDto?> GetMenuByIdAsync(int id)
        {
            Menu? menu = await _dbContext.Menus
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
                return null;

            return MenuMapper.ToSimpleDto(menu);
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
