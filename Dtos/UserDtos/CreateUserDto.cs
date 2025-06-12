namespace CesiZen_Backend.Dtos.UserDtos
{
    public record CreateUserDto(
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        bool Disabled,
        string Role
    );
}
