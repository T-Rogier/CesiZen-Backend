using CesiZen_Backend.Dtos.UserDtos;

namespace CesiZen_Backend.Services.UserService
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto command);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task UpdateUserAsync(int id, UpdateUserDto command);
        Task DeleteUserAsync(int id);
    }
}
