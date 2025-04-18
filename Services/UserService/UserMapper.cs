using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.UserService
{
    public class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto(
                user.Id,
                user.Username,
                user.Email,
                user.Disabled,
                user.Role.ToString()
            );
        }
    }
}
