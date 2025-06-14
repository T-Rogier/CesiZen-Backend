using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResultResponseDto> RegisterAsync(RegisterRequestDto registerDto);
        Task<AuthResultResponseDto> LoginAsync(LoginRequestDto loginDto);
        Task<AuthResultResponseDto> ExternalLoginAsync(ExternalLoginRequestDto dto);
        Task<AuthResultResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(User userId);
    }
}
