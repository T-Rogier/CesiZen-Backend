﻿using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.AuthService;
using CesiZen_Backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _AuthService;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService) : base(currentUserService)
        {
            _AuthService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            try
            {
                AuthResultResponseDto result = await _AuthService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                AuthResultResponseDto result = await _AuthService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [HttpPost("external/google")]
        public async Task<IActionResult> ExternalLoginGoogle([FromBody] ExternalLoginRequestDto dto)
        {
            try
            {
                AuthResultResponseDto result = await _AuthService.ExternalLoginAsync(dto);
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
                AuthResultResponseDto result = await _AuthService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(result);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            User user = await GetCurrentUserAsync();
            await _AuthService.LogoutAsync(user);
            return NoContent();
        }
    }
}
