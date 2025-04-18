namespace CesiZen_Backend.Dtos.UserDtos
{
    public record CreateUserDto(
        string Username,
        string Email,
        string Password,
        bool Disabled,
        string Role
    );
}
