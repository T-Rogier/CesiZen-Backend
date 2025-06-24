using CesiZen_Backend.Common.Options;
using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.AuthService;
using CesiZen_Backend.Services.PasswordHandler;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CesiZen_Backend.Tests.Services.Auth
{
    public class AuthService_GenerateAuthResult_Tests
    {
        [Fact]
        public async Task GenerateAuthResult_SetsRefreshTokenAndExpiryAndReturnsDtoWithCorrectProperties()
        {
            // Arrange
            var jwtOpts = Options.Create(new JwtOptions
            {
                Secret = "supersecretkey1234567890dddddddddddddd",
                Issuer = "testIssuer",
                Audience = "testAudience",
                AccessTokenExpirationMinutes = 30,
                RefreshTokenExpirationDays = 14
            });
            var logger = Mock.Of<ILogger<AuthService>>();
            var hasherMock = Mock.Of<IPasswordHasher>();

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            var service = new TestableAuthService(ctxMock.Object, logger, jwtOpts, hasherMock);

            var user = User.Create("username", "email@example.com", "pwdpwd", UserRole.User);

            // Act
            var result = await service.CallGenerateAuthResult(user);

            // Assert
            Assert.Equal("fixed-refresh", user.RefreshToken);
            Assert.True(user.RefreshTokenExpiryTime > DateTime.UtcNow);
            // Assert
            Assert.Equal("fixed-refresh", result.RefreshToken);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.AccessToken);
            var claims = token.Claims.ToList();
            Assert.Equal(user.Id.ToString(), token.Claims.First(c => c.Type == "nameid").Value);
            Assert.Equal("email@example.com", token.Claims.First(c => c.Type == "email").Value);
            Assert.Equal("User", token.Claims.First(c => c.Type == "role").Value);
            Assert.Equal("testIssuer", token.Issuer);
            Assert.Contains("testAudience", token.Audiences);

            var expectedExp = token.ValidFrom.AddMinutes(jwtOpts.Value.AccessTokenExpirationMinutes);
            Assert.True(Math.Abs((token.ValidTo - expectedExp).TotalSeconds) < 5);
        }

        private class TestableAuthService : AuthService
        {
            public TestableAuthService(
                CesiZenDbContext context,
                ILogger<AuthService> logger,
                IOptions<JwtOptions> options,
                IPasswordHasher hasher)
                : base(context, logger, options, hasher)
            {
            }

            public Task<AuthResultResponseDto> CallGenerateAuthResult(User user)
                => GenerateAuthResult(user);

            protected override string GenerateRefreshToken() => "fixed-refresh";
            protected override string ComputeSha256Hash(string raw) => raw;
        }
    }
}
