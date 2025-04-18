using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;

        public UserController(IUserService userService)
        {
            _UserService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto command)
        {
            var user = await _UserService.CreateUserAsync(command);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var users = await _UserService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _UserService.GetUserByIdAsync(id);
            return user is null ? NotFound(new { Message = $"User with ID {id} not found." }) : Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto command)
        {
            await _UserService.UpdateUserAsync(id, command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _UserService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
