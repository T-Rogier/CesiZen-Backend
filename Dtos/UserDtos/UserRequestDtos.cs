namespace CesiZen_Backend.Dtos.UserDtos
{
    public record CreateUserRequestDto(
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        string Role
    );

    public record UpdateUserRequestDto(
        string Username,
        string Password,
        string ConfirmPassword,
        bool Disabled,
        string Role
    );

    public record UserFilterRequestDto(
        string? Username,
        string? Email,
        bool? Disabled,
        string? Role,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        int PageNumber = 1,
        int PageSize = 10
    );
}
