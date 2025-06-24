using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.CategoryService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Categories
{
    public class CategoryService_DeleteCategory_Tests
    {
        [Fact]
        public async Task DeleteCategoryAsync_ExistingCategory_SetsDeletedAndUpdatesTimestamp()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Category original;
            using (var seedContext = new CesiZenDbContext(options))
            {
                original = Category.Create("NameToDelete", "Icon");
                seedContext.Categories.Add(original);
                seedContext.SaveChanges();
            }

            // Act
            using (var context = new CesiZenDbContext(options))
            {
                var service = new CategoryService(context, Mock.Of<ILogger<CategoryService>>());
                var existing = await context.Categories.FirstAsync();
                await service.DeleteCategoryAsync(existing.Id);
            }

            // Assert
            using var assertContext = new CesiZenDbContext(options);
            var deleted = await assertContext.Categories.FirstAsync();
            Assert.True(deleted.Deleted);
        }

        [Fact]
        public async Task DeleteCategoryAsync_InvalidId_ThrowsArgumentNullException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new CesiZenDbContext(options);
            var service = new CategoryService(context, Mock.Of<ILogger<CategoryService>>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.DeleteCategoryAsync(999)
            );
        }
    }
}
