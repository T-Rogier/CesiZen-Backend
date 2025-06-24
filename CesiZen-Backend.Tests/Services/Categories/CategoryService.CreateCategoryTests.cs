using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.CategoryService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Categories
{
    public class CategoryService_CreateCategory_Tests
    {
        [Fact]
        public async Task CreateCategoryAsync_ShouldAddCategoryAndReturnDto()
        {
            // Arrange
            var command = new CreateCategoryRequestDto("Relaxation", "link");

            var addedCategories = new List<Category>();
            var dbSetMock = MockDbSetHelper.CreateMockDbSet(addedCategories);

            dbSetMock
                .Setup(m => m.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Callback<Category, CancellationToken>((c, _) => addedCategories.Add(c))
                .Returns((Category c, CancellationToken _)
                    => new ValueTask<EntityEntry<Category>>((EntityEntry<Category>)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var dbContextMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            dbContextMock.Setup(db => db.Categories).Returns(dbSetMock.Object);
            dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var loggerMock = new Mock<ILogger<CategoryService>>();
            var service = new CategoryService(dbContextMock.Object, loggerMock.Object);

            // Act
            var result = await service.CreateCategoryAsync(command);

            // Assert
            dbSetMock.Verify(m => m.AddAsync(It.IsAny<Category>(), default), Times.Once);
            dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            var created = Assert.Single(addedCategories);
            Assert.Equal("Relaxation", created.Name);
            Assert.Equal("link", created.IconLink);

            Assert.Equal("Relaxation", result.Name);
        }

        [Theory]
        [InlineData(null, "icon")]
        [InlineData("", "icon")]
        [InlineData("Name", null)]
        [InlineData("Name", "")]
        public async Task CreateCategoryAsync_InvalidInputs_ThrowsArgumentException(string? name, string? iconLink)
        {
            // Arrange
            var command = new CreateCategoryRequestDto(name, iconLink);
            var dbSetMock = new Mock<DbSet<Category>>();

            dbSetMock
              .Setup(m => m.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
              .Returns((Category c, CancellationToken _)
                  => new ValueTask<EntityEntry<Category>>((EntityEntry<Category>)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var dbContextMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            dbContextMock.Setup(db => db.Categories).Returns(dbSetMock.Object);

            var service = new CategoryService(dbContextMock.Object ,Mock.Of<ILogger<CategoryService>>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCategoryAsync(command));

            dbSetMock.Verify(m => m.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCategoryAsync_WhenSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            var command = new CreateCategoryRequestDto("ValidName", "ValidIcon");
            var addedCategories = new List<Category>();

            // Mock du DbSet pour capturer AddAsync
            var dbSetMock = new Mock<DbSet<Category>>();
            dbSetMock
              .Setup(m => m.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
              .Callback<Category, CancellationToken>((c, _) => addedCategories.Add(c))
              .Returns((Category c, CancellationToken _)
                  => new ValueTask<EntityEntry<Category>>((EntityEntry<Category>)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var dbContextMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            dbContextMock.Setup(db => db.Categories).Returns(dbSetMock.Object);

            dbContextMock
              .Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
              .ThrowsAsync(new DbUpdateException("Simulated failure"));

            var service = new CategoryService(dbContextMock.Object, Mock.Of<ILogger<CategoryService>>());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => service.CreateCategoryAsync(command));

            dbSetMock.Verify(m => m.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
                             Times.Once);

            Assert.Single(addedCategories);
            Assert.Equal("ValidName", addedCategories[0].Name);
            Assert.Equal("ValidIcon", addedCategories[0].IconLink);
        }
    }
}
