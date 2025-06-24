using Bogus.DataSets;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.MenuService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Menus
{
    public class MenuService_GetMenuHierarchy_Tests
    {
        private static DbContextOptions<CesiZenDbContext> CreateNewContextOptions()
            => new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        [Fact]
        public async Task GetMenuHierarchyAsync_NoRootMenus_ThrowsArgumentNullException()
        {
            // Arrange
            var options = CreateNewContextOptions();
            await using (var ctx = new CesiZenDbContext(options)){ }

            // Act & Assert
            await using var context = new CesiZenDbContext(options);
            var service = new MenuService(context, Mock.Of<ILogger<MenuService>>());

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.GetMenuHierarchyAsync()
            );
            Assert.Contains("Aucun menu trouvé", ex.Message);
        }

        [Fact]
        public async Task GetMenuHierarchyAsync_TwoLevelsHierarchy_ReturnsAllLevelsDto()
        {
            // Arrange
            var options = CreateNewContextOptions();
            int rootId, childId, rootArticleId, childArticleId;

            await using (var seedCtx = new CesiZenDbContext(options))
            {
                var root = Menu.Create("Root", hierarchyLevel: 0);
                seedCtx.Menus.Add(root);
                await seedCtx.SaveChangesAsync();
                rootId = root.Id;

                var child = Menu.Create("Child", hierarchyLevel: 1, parentId: rootId);
                seedCtx.Menus.Add(child);
                await seedCtx.SaveChangesAsync();
                childId = child.Id;

                var artRoot = Article.Create("ARoot", "RootContent", root);
                var artChild = Article.Create("AChild", "ChildContent", child);
                seedCtx.Articles.AddRange(artRoot, artChild);
                await seedCtx.SaveChangesAsync();
                rootArticleId = artRoot.Id;
                childArticleId = artChild.Id;
            }

            // Act
            await using var ctx = new CesiZenDbContext(options);
            var service = new MenuService(ctx, Mock.Of<ILogger<MenuService>>());
            var result = (await service.GetMenuHierarchyAsync()).ToList();

            // Assert
            Assert.Single(result);
            var rootDto = result[0];
            Assert.Equal(rootId, rootDto.Id);
            Assert.Equal("Root", rootDto.Title);
            Assert.Equal(0, rootDto.HierarchyLevel);

            Assert.NotNull(rootDto.ChildArticles);
            Assert.Collection(rootDto.ChildArticles,
                dto =>
                {
                    Assert.Equal(rootArticleId, dto.Id);
                    Assert.Equal("ARoot", dto.Title);
                }
            );

            Assert.NotNull(rootDto.ChildMenus);
            Assert.Single(rootDto.ChildMenus);
            var childDto = rootDto.ChildMenus.Single();
            Assert.Equal(childId, childDto.Id);
            Assert.Equal("Child", childDto.Title);
            Assert.Equal(1, childDto.HierarchyLevel);

            Assert.NotNull(childDto.ChildArticles);
            Assert.Collection(childDto.ChildArticles,
                dto =>
                {
                    Assert.Equal(childArticleId, dto.Id);
                    Assert.Equal("AChild", dto.Title);
                }
            );

            Assert.True(childDto.ChildMenus == null || !childDto.ChildMenus.Any());
        }

        [Fact]
        public async Task GetMenuHierarchyAsync_ThreeLevelsHierarchy_ReturnsAllLevelsDto()
        {
            // Arrange
            var options = CreateNewContextOptions();
            int rootId, childId, grandId;
            int rootArtId, childArtId, grandArtId;

            await using (var seedCtx = new CesiZenDbContext(options))
            {
                var root = Menu.Create("Root", hierarchyLevel: 0);
                seedCtx.Menus.Add(root);
                await seedCtx.SaveChangesAsync();
                rootId = root.Id;

                var child = Menu.Create("Child", hierarchyLevel: 1, parentId: rootId);
                seedCtx.Menus.Add(child);
                await seedCtx.SaveChangesAsync();
                childId = child.Id;

                var grand = Menu.Create("Grand", hierarchyLevel: 2, parentId: childId);
                seedCtx.Menus.Add(grand);
                await seedCtx.SaveChangesAsync();
                grandId = grand.Id;

                var artRoot = Article.Create("ARoot", "Content Root", root);
                var artChild = Article.Create("AChild", "Content Child", child);
                var artGrand = Article.Create("AGrand", "Content Grand", grand);
                seedCtx.Articles.AddRange(artRoot, artChild, artGrand);
                await seedCtx.SaveChangesAsync();

                rootArtId = artRoot.Id;
                childArtId = artChild.Id;
                grandArtId = artGrand.Id;
            }

            // Act
            await using var ctx = new CesiZenDbContext(options);
            var service = new MenuService(ctx, Mock.Of<ILogger<MenuService>>());
            var result = (await service.GetMenuHierarchyAsync()).ToList();

            // Assert
            Assert.Single(result);
            var rootDto = result[0];
            Assert.Equal(rootId, rootDto.Id);
            Assert.Equal("Root", rootDto.Title);
            Assert.Equal(0, rootDto.HierarchyLevel);

            Assert.NotNull(rootDto.ChildArticles);
            Assert.Collection(rootDto.ChildArticles,
                a => {
                    Assert.Equal(rootArtId, a.Id);
                    Assert.Equal("ARoot", a.Title);
                }
            );

            Assert.NotNull(rootDto.ChildMenus);
            Assert.Single(rootDto.ChildMenus);
            var childDto = rootDto.ChildMenus.Single();
            Assert.Equal(childId, childDto.Id);
            Assert.Equal("Child", childDto.Title);
            Assert.Equal(1, childDto.HierarchyLevel);

            Assert.NotNull(childDto.ChildArticles);
            Assert.Collection(childDto.ChildArticles,
                a => {
                    Assert.Equal(childArtId, a.Id);
                    Assert.Equal("AChild", a.Title);
                }
            );

            Assert.NotNull(childDto.ChildMenus);
            Assert.Single(childDto.ChildMenus);
            var grandDto = childDto.ChildMenus.Single();
            Assert.Equal(grandId, grandDto.Id);
            Assert.Equal("Grand", grandDto.Title);
            Assert.Equal(2, grandDto.HierarchyLevel);

            Assert.NotNull(grandDto.ChildArticles);
            Assert.Collection(grandDto.ChildArticles,
                a => {
                    Assert.Equal(grandArtId, a.Id);
                    Assert.Equal("AGrand", a.Title);
                }
            );

            Assert.True(grandDto.ChildMenus == null || !grandDto.ChildMenus.Any());
        }
    }
}
