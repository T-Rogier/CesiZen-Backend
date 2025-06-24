namespace CesiZen_Backend.Dtos.AuthDtos
{
    public record AuthResultResponseDto(
        string AccessToken, 
        string RefreshToken, 
        string Username, 
        string Role
    );
}