namespace CesiZen_Backend.Dtos.UserDtos
{
    public record UserDto(
        int Id,
        string Username,
        string Email,
        bool Disabled,
        string Role
    );
}
