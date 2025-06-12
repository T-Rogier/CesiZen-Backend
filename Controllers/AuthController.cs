using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.AuthService;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _AuthService;
        private readonly IValidator<LoginDto> _Validator;

        public AuthController(IAuthService authService, IValidator<LoginDto> validator)
        {
            _AuthService = authService;
            _Validator = validator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var validationResult = await _Validator.ValidateAsync(loginDto);
                if (!validationResult.IsValid)
                {
                    return (IActionResult)Results.ValidationProblem(validationResult.ToDictionary());
                }
                var result = await _AuthService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [HttpPost("external/google")]
        public async Task<IActionResult> ExternalLoginGoogle([FromBody] ExternalLoginDto dto)
        {
            try
            {
                var result = await _AuthService.ExternalLoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                var result = await _AuthService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(result);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok(new { Message = "You are authenticated!", User = User.Identity?.Name });
        }
    }
}
