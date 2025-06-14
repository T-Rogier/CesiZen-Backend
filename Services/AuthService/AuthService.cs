using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Options;
using CesiZen_Backend.Persistence;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CesiZen_Backend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly CesiZenDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtOptions _jwtOptions;

        public AuthService(CesiZenDbContext context, ILogger<AuthService> logger, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _logger = logger;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<AuthResultResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            User? existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser is not null)
                throw new InvalidOperationException("Un utilisateur avec cet email existe déjà.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            User user = User.Create(
                registerDto.Username,
                registerDto.Email,
                hashedPassword,
                UserRole.User
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GenerateAuthResult(user);
        }

        public async Task<AuthResultResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            User? user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null || user.Password is null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (user.Disabled)
            {
                throw new UnauthorizedAccessException("Ce compte est désactivé.");
            }
            return await GenerateAuthResult(user);
        }

        public async Task<AuthResultResponseDto> ExternalLoginAsync(ExternalLoginRequestDto dto)
        {
            return dto.Provider switch
            {
                "Google" => await HandleGoogleLogin(dto),
                _ => throw new NotSupportedException("Provider not supported.")
            };
        }

        public async Task<AuthResultResponseDto> RefreshTokenAsync(string refreshToken)
        {
            string hashedToken = ComputeSha256Hash(refreshToken);
            User? user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == hashedToken);

            if (user is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new SecurityTokenException("Invalid or expired refresh token");

            return await GenerateAuthResult(user);
        }

        public async Task LogoutAsync(User user)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _context.SaveChangesAsync();
        }

        private async Task<AuthResultResponseDto> GenerateAuthResult(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ];

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);

            string refreshToken = GenerateRefreshToken();
            string hashedToken = ComputeSha256Hash(refreshToken);

            user.RefreshToken = hashedToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

            await _context.SaveChangesAsync();

            return new AuthResultResponseDto
            (
                jwt,
                refreshToken,
                user.Username,
                user.Role.ToString()
            );
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[64];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string ComputeSha256Hash(string rawData)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }

        private async Task<AuthResultResponseDto> HandleGoogleLogin(ExternalLoginRequestDto dto)
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Provider == "Google" && u.ProviderId == payload.Subject);

            if (user is null)
            {
                user = User.Create(payload.Name, payload.Email, null, UserRole.User, "Google", payload.Subject);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return await GenerateAuthResult(user);
        }
    }
}
