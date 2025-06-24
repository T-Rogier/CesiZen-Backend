using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.CategoryService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Categories
{
    public class CategoryService_GetCategoryById_Tests
    {
        [Fact]
        public async Task GetCategoryByIdAsync_ExistingId_ReturnsDto()
        {
            // Arrange
            var category = Category.Create("TestName", "TestIcon");

            var data = new List<Category> { category };
            var ctx = ContextMockHelper.CreateCategoryContextMock(data);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());

            // Act
            var dto = await service.GetCategoryByIdAsync(0);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("TestName", dto.Name);
            Assert.Equal("TestIcon", dto.IconLink);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var category = Category.Create("TestName", "TestIcon");
            var data = new List<Category> { category };
            var ctx = ContextMockHelper.CreateCategoryContextMock(data);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());

            // Act
            var dto = await service.GetCategoryByIdAsync(42);

            // Assert
            Assert.Null(dto);
        }
    }
}
