using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _AuthService;

        public AuthController(IAuthService authService)
        {
            _AuthService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                AuthResultDto result = await _AuthService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                AuthResultDto result = await _AuthService.LoginAsync(loginDto);
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
                AuthResultDto result = await _AuthService.ExternalLoginAsync(dto);
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
                AuthResultDto result = await _AuthService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(result);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [Authorize()]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok(new { Message = "You are authenticated!", User = User.Identity?.Name });
        }
    }
}
