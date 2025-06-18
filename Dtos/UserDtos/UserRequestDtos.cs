using CesiZen_Backend.Models;

namespace CesiZen_Backend.Dtos.UserDtos
{
    public record CreateUserRequestDto(
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        UserRole Role
    );

    public record UpdateUserRequestDto(
        string Username,
        string Password,
        string ConfirmPassword,
        bool Disabled,
        UserRole Role
    );

    public record UserFilterRequestDto(
        string? Username,
        string? Email,
        bool? Disabled,
        UserRole? Role,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        int PageNumber = 1,
        int PageSize = 10
    );
}
