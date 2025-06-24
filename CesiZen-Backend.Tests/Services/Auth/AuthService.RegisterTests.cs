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

namespace CesiZen_Backend.Tests.Services.Auth
{
    public class AuthService_Register_Tests
    {
        private static CesiZenDbContext CreateUserContextMock(List<User> users, out List<User> addedUsers)
        {
            var queryable = users.AsQueryable();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.As<IAsyncEnumerable<User>>()
                     .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                     .Returns((CancellationToken ct) => new TestAsyncEnumerator<User>(queryable.GetEnumerator()));
            dbSetMock.As<IQueryable<User>>().Setup(m => m.Provider)
                     .Returns(new TestAsyncQueryProvider<User>(queryable.Provider));
            dbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            var capturedUsers = new List<User>();
            dbSetMock.Setup(m => m.Add(It.IsAny<User>()))
                     .Callback<User>(capturedUsers.Add);

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(db => db.Users).Returns(dbSetMock.Object);
            ctxMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            addedUsers = capturedUsers;
            return ctxMock.Object;
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var passwordHasher = Mock.Of<IPasswordHasher>();
            var existing = User.Create("user1", "user1@example.com", passwordHasher.HashPassword("pwd1"), UserRole.User);
            var context = CreateUserContextMock(new List<User> { existing }, out _);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            
            var service = new AuthService(context, logger, jwtOptions, passwordHasher);
            var dto = new RegisterRequestDto("user1", "user1@example.com", "pwd1", "pwd1");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterAsync(dto)
            );
            Assert.Equal("Un utilisateur avec cet email existe déjà.", ex.Message);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_AddsUserAndReturnsAuthResult()
        {
            // Arrange
            var context = CreateUserContextMock(new List<User>(), out var addedUsers);
            var logger = Mock.Of<ILogger<AuthService>>();
            var jwtOptions = Options.Create(new JwtOptions());
            var hasherMock = new Mock<IPasswordHasher>();
            hasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                      .Returns((string pwd) => "hashed-" + pwd);

            var expectedAuth = new AuthResultResponseDto("token123", "refresh123", "user1", "Administrateur");

            var service = new TestableAuthService(context, logger, jwtOptions, hasherMock.Object, expectedAuth);
            var dto = new RegisterRequestDto("user1", "user1@example.com", "pwd1", "pwd1");

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.Same(expectedAuth, result);
            Assert.Single(addedUsers);
            var added = addedUsers.Single();
            Assert.Equal("user1", added.Username);
            Assert.Equal("user1@example.com", added.Email);
            Assert.Equal("hashed-pwd1", added.Password);
            Assert.Equal(UserRole.User, added.Role);
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
