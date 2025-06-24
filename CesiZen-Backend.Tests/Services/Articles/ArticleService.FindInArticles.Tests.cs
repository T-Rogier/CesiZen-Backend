using CesiZen_Backend.Dtos.ArticleDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.Articleservice;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Reflection;

namespace CesiZen_Backend.Tests.Services.Articles
{
    public class ArticleService_FindInArticles_Tests
    {
        [Fact]
        public async Task FindInArticleAsync_WithQuery_FiltersOrdersAndPaginates()
        {
            // Arrange
            var menu = Menu.Create("M", 0);
            var a1 = Article.Create("Apple pie", "Delicious apple pie", menu);
            var a2 = Article.Create("Banana bread", "Best banana bread recipe", menu);
            var a3 = Article.Create("Cherry tart", "Try our apple and cherry tart", menu);
            var all = new List<Article> { a1, a2, a3 };

            var ctx = ContextMockHelper.CreateContextWithArticles(all);
            var service = new ArticleService(ctx, Mock.Of<ILogger<ArticleService>>());
            var filter = new FindInArticleRequestDto("apple", PageNumber: 1, PageSize: 10);

            // Act
            var result = await service.FindInArticleAsync(filter);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Articles.Count());

            foreach (var article in result.Articles)
            {
                Assert.True(
                    article.Title.Contains("apple", StringComparison.OrdinalIgnoreCase)
                 || article.Content.Contains("apple", StringComparison.OrdinalIgnoreCase),
                    $"Article Id={article.Id} ne contient pas 'apple'."
                );
            }
        }

        [Fact]
        public async Task FindInArticleAsync_NoQuery_ReturnsAllOrderedAndPaged()
        {
            // Arrange
            var menu = Menu.Create("M", 0);
            var articles = Enumerable.Range(1, 5)
                .Select(i =>
                {
                    var a = Article.Create($"Title{i}", $"Content{i}", menu);
                    SetPrivateId(a, i);
                    return a;
                })
                .ToList();

            var ctx = ContextMockHelper.CreateContextWithArticles(articles);
            var service = new ArticleService(ctx, Mock.Of<ILogger<ArticleService>>());
            var filter = new FindInArticleRequestDto(null, PageNumber: 2, PageSize: 2);

            // Act
            var result = await service.FindInArticleAsync(filter);

            // Assert
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Articles.Count());
            // Expect Ids [3,2]
            Assert.Equal(3, result.Articles.ToArray()[0].Id);
            Assert.Equal(2, result.Articles.ToArray()[1].Id);
        }

        [Fact]
        public async Task FindInArticleAsync_NoMatches_ReturnsEmpty()
        {
            // Arrange
            var menu = Menu.Create("M", 0);
            var articles = new List<Article> { Article.Create("Foo", "Bar", menu) };

            var ctx = ContextMockHelper.CreateContextWithArticles(articles);
            var service = new ArticleService(ctx, Mock.Of<ILogger<ArticleService>>());
            var filter = new FindInArticleRequestDto("nonexistent", PageNumber: 1, PageSize: 10);

            // Act
            var result = await service.FindInArticleAsync(filter);

            // Assert
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Articles);
        }

        static void SetPrivateId<T>(T entity, int id)
        {
            var prop = typeof(T).GetProperty("Id",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            prop.SetValue(entity, id);
        }
    }
}
