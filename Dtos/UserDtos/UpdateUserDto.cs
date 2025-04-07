namespace CesiZen_Backend.Dtos.UserDtos
{
    public record UpdateUserDto(string Identifiant, string Email, string Password, bool Disabled, string Role);

}
