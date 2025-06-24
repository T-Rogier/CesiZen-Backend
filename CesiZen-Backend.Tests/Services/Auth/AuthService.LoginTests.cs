using CesiZen_Backend.Common.Options;
using CesiZen_Backend.Dtos.AuthDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.AuthService;
using CesiZen_Backend.Services.PasswordHandler;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CesiZen_Backend.Tests.Services.Auth
{
    public class AuthService_Login_Tests
    {
        [Fact]
        public async Task LoginAsync_NonExistingUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = ContextMockHelper.CreateUserContextMock([]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = Mock.Of<IPasswordHasher>();
            var service = new AuthService(context, logger, jwtOptions, hasherMock);
            var dto = new LoginRequestDto(Email: "user1@example.com", Password: "pwd");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(dto));
            Assert.Equal("Invalid credentials.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_PasswordMismatch_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = User.Create("user", "user1@example.com", "stored-hash", UserRole.User);
            var context = ContextMockHelper.CreateUserContextMock([user]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = new Mock<IPasswordHasher>();
            hasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(false);
            var service = new AuthService(context, logger, jwtOptions, hasherMock.Object);
            var dto = new LoginRequestDto(Email: "user1@example.com", Password: "wrongpwd");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(dto));
            Assert.Equal("Invalid credentials.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_DisabledUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = User.Create("user", "user1@example.com", "stored-hash", UserRole.User);
            user.Disable();
            var context = ContextMockHelper.CreateUserContextMock([user]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = new Mock<IPasswordHasher>();
            hasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(true);
            var service = new AuthService(context, logger, jwtOptions, hasherMock.Object);
            var dto = new LoginRequestDto(Email: "user1@example.com", Password: "pwd");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(dto));
            Assert.Equal("Ce compte est désactivé.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResult()
        {
            // Arrange
            var user = User.Create("user", "user1@example.com", "stored-hash", UserRole.User);
            var context = ContextMockHelper.CreateUserContextMock([user]);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = new Mock<IPasswordHasher>();
            hasherMock.Setup(h => h.Verify("goodpwd", "stored-hash")).Returns(true);

            var expectedAuth = new AuthResultResponseDto("token123", "refresh123", "user1", "Administrateur");
            var service = new TestableAuthService(context, logger, jwtOptions, hasherMock.Object, expectedAuth);
            var dto = new LoginRequestDto(Email: "user1@example.com", Password: "goodpwd");

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            Assert.Same(expectedAuth, result);
        }

        private class TestableAuthService : AuthService
        {
            private readonly AuthResultResponseDto _authResult;

            public TestableAuthService(
                CesiZenDbContext context,
                ILogger<AuthService> logger,
                IOptions<JwtOptions> jwtOptions,
                IPasswordHasher hasher,
                AuthResultResponseDto authResult)
                : base(context, logger, jwtOptions, hasher)
            {
                _authResult = authResult;
            }
            protected override Task<AuthResultResponseDto> GenerateAuthResult(User user)
                    => Task.FromResult(_authResult);
        }
    }
}
