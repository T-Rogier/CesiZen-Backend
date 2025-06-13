using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _UserService;

        public UserController(IUserService userService, ICurrentUserService currentUserService) : base(currentUserService)
        {
            _UserService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto command)
        {
            UserDto user = await _UserService.CreateUserAsync(command);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            IEnumerable<UserDto> users = await _UserService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize()]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            User user = await CurrentUserAsync();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            UserDto? user = await _UserService.GetUserByIdAsync(id);
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
