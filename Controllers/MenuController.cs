using CesiZen_Backend.Dtos.MenuDtos;
using CesiZen_Backend.Services.MenuService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _MenuService;

        public MenuController(IMenuService menuService)
        {
            _MenuService = menuService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequestDto command)
        {
            SimpleMenuResponseDto menu = await _MenuService.CreateMenuAsync(command);
            return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            IEnumerable<SimpleMenuResponseDto> menus = await _MenuService.GetAllMenusAsync();
            return Ok(menus);
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetMenuHierarchy()
        {
            IEnumerable<FullMenuResponseDto> menus = await _MenuService.GetMenuHierarchyAsync();
            return Ok(menus);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            SimpleMenuResponseDto? menu = await _MenuService.GetMenuByIdAsync(id);
            return menu is null ? NotFound(new { Message = $"Menu with ID {id} not found." }) : Ok(menu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuRequestDto command)
        {
            await _MenuService.UpdateMenuAsync(id, command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            await _MenuService.DeleteMenuAsync(id);
            return NoContent();
        }
    }
}
