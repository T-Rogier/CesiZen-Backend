namespace CesiZen_Backend.Dtos.UserDtos
{
    public record UserDto(int Id, string Identifiant, string Email, bool Disabled, string Role);

}
