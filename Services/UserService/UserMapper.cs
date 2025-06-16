using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.UserService
{
    public class UserMapper
    {
        public static FullUserResponseDto ToFullDto(User user)
        {
            return new FullUserResponseDto(
                user.Id,
                user.Username,
                user.Email,
                user.Disabled,
                user.Role.GetDisplayName()
            );
        }

        public static SimpleUserResponseDto ToSimpleDto(User user)
        {
            return new SimpleUserResponseDto(
                user.Id,
                user.Username
            );
        }

        public static UserListResponseDto ToListDto(List<User> users, int totalCount, int pageNumber = 1, int pageSize = 10)
        {
            return new UserListResponseDto(
                users.Select(ToSimpleDto),
                pageNumber,
                pageSize,
                totalCount,
                totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0)
            );
        }

    }
}
