using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.UserService
{
    public interface ICurrentUserService
    {
        Task<User?> TryGetUserAsync();
        Task<User> GetUserAsync();
        Task<FullUserResponseDto> GetUserProfileAsync();
    }
}
