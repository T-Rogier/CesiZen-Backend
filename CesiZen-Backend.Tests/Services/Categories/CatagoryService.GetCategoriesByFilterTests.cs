using CesiZen_Backend.Dtos.CategoryDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.CategoryService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Categories
{
    public class CategoryService_GetCategoriesByFilter_Tests
    {
        [Fact]
        public async Task NoFilter_DefaultsToPage1AndSize1_ReturnsSingleFirstItem()
        {
            // Arrange
            var categories = new List<Category>
            {
                Category.Create("A", "x"),
                Category.Create("B", "x"),
                Category.Create("C", "x"),
            };
            var ctx = ContextMockHelper.CreateCategoryContextMock(categories);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());

            var filter = new CategoryFilterRequestDto(Name: null, PageNumber: 0, PageSize: 0);

            // Act
            var dto = await service.GetCategoriesByFilterAsync(filter);

            // Assert
            Assert.Equal(3, dto.TotalCount);
            Assert.Equal(1, dto.PageNumber);
            Assert.Equal(1, dto.PageSize);
            Assert.Single(dto.Categories);
            Assert.Equal("A", dto.Categories.First().Name);
        }

        [Fact]
        public async Task NameFilter_NonCaseInsensitive_ReturnsMatchingItems()
        {
            // Arrange
            var categories = new List<Category>
            {
                Category.Create("Alpha", "x"),
                Category.Create("beta",  "x"),
                Category.Create("Gamma", "x"),
                Category.Create("delta", "x"),
            };
            var ctx = ContextMockHelper.CreateCategoryContextMock(categories);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());
            var filter = new CategoryFilterRequestDto(Name: "A", PageNumber: 1, PageSize: 10);

            // Act
            var dto = await service.GetCategoriesByFilterAsync(filter);

            // Assert
            Assert.Equal(4, dto.TotalCount);
            Assert.Equal(1, dto.PageNumber);
            Assert.Equal(10, dto.PageSize);
            Assert.Collection(dto.Categories,
                c => Assert.Equal("Alpha", c.Name),
                c => Assert.Equal("beta", c.Name),
                c => Assert.Equal("Gamma", c.Name),
                c => Assert.Equal("delta", c.Name));
        }

        [Fact]
        public async Task Paging_Page2Size2_ReturnsCorrectSubset()
        {
            // Arrange
            var categories = new List<Category>
            {
                Category.Create("1", "x"),
                Category.Create("2", "x"),
                Category.Create("3", "x"),
                Category.Create("4", "x"),
                Category.Create("5", "x"),
            };
            var ctx = ContextMockHelper.CreateCategoryContextMock(categories);
            var service = new CategoryService(ctx, Mock.Of<ILogger<CategoryService>>());
            var filter = new CategoryFilterRequestDto(Name: string.Empty, PageNumber: 2, PageSize: 2);

            // Act
            var dto = await service.GetCategoriesByFilterAsync(filter);

            // Assert
            Assert.Equal(5, dto.TotalCount);
            Assert.Equal(2, dto.PageNumber);
            Assert.Equal(2, dto.PageSize);
            Assert.Collection(dto.Categories,
                c => Assert.Equal("3", c.Name),
                c => Assert.Equal("4", c.Name));
        }
    }
}
