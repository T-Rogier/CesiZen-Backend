using CesiZen_Backend.Models;
using CesiZen_Backend.Services.CategoryService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Categories
{
    public class CategoryService_GetAllCategories_Tests
    {
        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnOnlyNotDeleted()
        {
            // Arrange
            var c1 = Category.Create("A", "iconA");
            var c2 = Category.Create("B", "iconB");
            c2.Delete();
            var c3 = Category.Create("C", "iconC");

            var categories = new List<Category> { c1, c2, c3 };
            var ctx = ContextMockHelper.CreateCategoryContextMock(categories);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());

            // Act
            var dto = await service.GetAllCategoriesAsync();

            // Assert
            Assert.Equal(2, dto.TotalCount);
            Assert.Collection(dto.Categories,
                item => Assert.Equal("A", item.Name),
                item => Assert.Equal("C", item.Name)
            );
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WhenAllDeleted_ShouldReturnEmpty()
        {
            // Arrange
            var c1 = Category.Create("X", "iconX");
            c1.Delete();
            var c2 = Category.Create("Y", "iconY");
            c2.Delete();

            var categories = new List<Category> { c1, c2 };
            var ctx = ContextMockHelper.CreateCategoryContextMock(categories);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());

            // Act
            var dto = await service.GetAllCategoriesAsync();

            // Assert
            Assert.Equal(0, dto.TotalCount);
            Assert.Empty(dto.Categories);
        }

    }
}
