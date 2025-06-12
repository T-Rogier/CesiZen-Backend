namespace CesiZen_Backend.Dtos.AuthDtos
{
    public record LoginDto(string Email, string Password, string ConfirmPassword);
    public record ExternalLoginDto(string Provider, string IdToken);
    public record RefreshTokenRequestDto(string RefreshToken);
    public record AuthResultDto(string AccessToken, string RefreshToken, string Username, string Role);
}