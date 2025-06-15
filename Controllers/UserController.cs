using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Filters;
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

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto command)
        {
            FullUserResponseDto user = await _UserService.CreateUserAsync(command);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            UserListResponseDto users = await _UserService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersByFilter([FromQuery] UserFilterRequestDto filter)
        {
            UserListResponseDto users = await _UserService.GetUsersByFilterAsync(filter);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            User user = await CurrentUserAsync();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            FullUserResponseDto? user = await _UserService.GetUserByIdAsync(id);
            return user is null ? NotFound(new { Message = $"User with ID {id} not found." }) : Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto command)
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
