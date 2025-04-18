namespace CesiZen_Backend.Dtos.UserDtos
{
    public record UpdateUserDto(
        string Username,
        string Email,
        string Password,
        bool Disabled,
        string Role
    );
}
