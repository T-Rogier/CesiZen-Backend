using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> ExternalLoginAsync(ExternalLoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    }
}
