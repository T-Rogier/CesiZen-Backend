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
        string Email,
        string Password,
        bool Disabled,
        string Role
    );
}
