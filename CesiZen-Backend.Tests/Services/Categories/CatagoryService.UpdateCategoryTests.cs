using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.CategoryService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Categories
{
    public class CategoryService_UpdateCategory_Tests
    {
        [Fact]
        public async Task UpdateCategoryAsync_ExistingCategory_UpdatesPropertiesAndSaves()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Category original;
            using (var seedContext = new CesiZenDbContext(options))
            {
                original = Category.Create("OldName", "OldIcon");
                seedContext.Categories.Add(original);
                seedContext.SaveChanges();
            }

            // Act
            using (var context = new CesiZenDbContext(options))
            {
                var service = new CategoryService(context, Mock.Of<ILogger<CategoryService>>());
                var existing = await context.Categories.FirstAsync();
                var command = new UpdateCategoryRequestDto("NewName", "NewIcon");
                await service.UpdateCategoryAsync(existing.Id, command);
            }

            // Assert
            using var assertContext = new CesiZenDbContext(options);
            var updated = await assertContext.Categories.FirstAsync();
            Assert.Equal("NewName", updated.Name);
            Assert.Equal("NewIcon", updated.IconLink);
        }



        [Fact]
        public async Task UpdateCategoryAsync_InvalidId_ThrowsArgumentNullException()
        {
            // Arrange
            var dbSetMock = new Mock<DbSet<Category>>();
            dbSetMock
              .Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
              .Returns((object[] ids, CancellationToken ct)
                  => new ValueTask<Category?>((Category?)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(db => db.Categories).Returns(dbSetMock.Object);

            var service = new CategoryService(ctxMock.Object, Mock.Of<ILogger<CategoryService>>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.UpdateCategoryAsync(123, new UpdateCategoryRequestDto("n", "i"))
            );
        }
    }
}
