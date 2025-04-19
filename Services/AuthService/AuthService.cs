using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;

namespace CesiZen_Backend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly CesiZenDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _config;

        public AuthService(CesiZenDbContext context, ILogger<AuthService> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || user.Password == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (user.Disabled)
            {
                throw new UnauthorizedAccessException("Ce compte est désactivé.");
            }
            return await GenerateAuthResult(user);
        }

        public async Task<AuthResultDto> ExternalLoginAsync(ExternalLoginDto dto)
        {
            if (dto.Provider == "Google")
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Provider == "Google" && u.ProviderId == payload.Subject);

                if (user == null)
                {
                    user = User.Create(payload.Name, payload.Email, null, UserRole.User, "Google", payload.Subject);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return await GenerateAuthResult(user);
            }

            throw new NotSupportedException("Provider not supported.");
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new SecurityTokenException("Invalid or expired refresh token");

            return await GenerateAuthResult(user);
        }

        private async Task<AuthResultDto> GenerateAuthResult(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new AuthResultDto
            (
                jwt,
                refreshToken,
                user.Username,
                user.Role.ToString()
            );
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
