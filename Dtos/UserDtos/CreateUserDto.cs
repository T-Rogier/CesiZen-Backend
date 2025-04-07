namespace CesiZen_Backend.Dtos.UserDtos
{
    public record CreateUserDto(string Identifiant, string Email, string Password, bool Disabled, string Role);

}
