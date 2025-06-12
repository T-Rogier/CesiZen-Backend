using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Services.UserService;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;
        private readonly IValidator<CreateUserDto> _Validator;

        public UserController(IUserService userService, IValidator<CreateUserDto> validator)
        {
            _UserService = userService;
            _Validator = validator;
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
