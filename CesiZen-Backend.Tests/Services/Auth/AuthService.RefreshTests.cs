using CesiZen_Backend.Common.Options;
using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.AuthService;
using CesiZen_Backend.Services.PasswordHandler;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace CesiZen_Backend.Tests.Services.Auth
{
    public class AuthService_Refresh_Tests
    {
        [Fact]
        public async Task RefreshTokenAsync_InvalidOrMissingToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var context = ContextMockHelper.CreateUserContextMock([]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = Mock.Of<IPasswordHasher>();
            var service = new AuthService(context, logger, jwtOptions, hasherMock);
            var badToken = "nonexistent";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<SecurityTokenException>(
                () => service.RefreshTokenAsync(badToken));
            Assert.Equal("Invalid or expired refresh token", ex.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ExpiredToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var rawToken = "mytoken";
            var user = User.Create("user", "u@ex.com", "pwdpwd", UserRole.User);
            user.RefreshToken = rawToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-5);

            var context = ContextMockHelper.CreateUserContextMock([user]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = Mock.Of<IPasswordHasher>();
            var service = new TestableAuthService(context, logger, jwtOptions, hasherMock, token => token);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<SecurityTokenException>(
                () => service.RefreshTokenAsync(rawToken));
            Assert.Equal("Invalid or expired refresh token", ex.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsAuthResult()
        {
            // Arrange
            var rawToken = "validtoken";
            var user = User.Create("user", "u@ex.com", "pwdpwd", UserRole.User);
            user.RefreshToken = rawToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1);

            var context = ContextMockHelper.CreateUserContextMock([user]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = Mock.Of<IPasswordHasher>();
            var expectedAuth = new AuthResultResponseDto("token123", "refresh123", "user1", "Administrateur");
            var service = new TestableAuthService(context, logger, jwtOptions, hasherMock, token => token, expectedAuth);

            // Act
            var result = await service.RefreshTokenAsync(rawToken);

            // Assert
            Assert.Same(expectedAuth, result);
        }

        private class TestableAuthService : AuthService
        {
            private readonly AuthResultResponseDto _authResult;
            private readonly Func<string, string> _hashToken;

            public TestableAuthService(
                CesiZenDbContext context,
                ILogger<AuthService> logger,
                IOptions<JwtOptions> jwtOptions,
                IPasswordHasher hasher,
                Func<string, string> hashToken,
                AuthResultResponseDto authResult = null!)
                : base(context, logger, jwtOptions, hasher)
            {
                _hashToken = hashToken;
                _authResult = authResult;
            }
            protected override string ComputeSha256Hash(string raw)
                    => _hashToken(raw);
            protected override Task<AuthResultResponseDto> GenerateAuthResult(User user)
                    => Task.FromResult(_authResult);
        }
    }
}
