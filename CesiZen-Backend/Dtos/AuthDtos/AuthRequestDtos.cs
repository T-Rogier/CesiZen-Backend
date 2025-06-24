namespace CesiZen_Backend.Dtos.AuthDtos
{
    public record LoginRequestDto(
        string Email, 
        string Password
    );

    public record RegisterRequestDto(
        string Username, 
        string Email, 
        string Password, 
        string ConfirmPassword
    );

    public record ExternalLoginRequestDto(
        string Provider, 
        string IdToken
    );

    public record RefreshTokenRequestDto(string RefreshToken);
}