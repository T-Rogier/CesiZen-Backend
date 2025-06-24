using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CesiZen_Backend.Tests.Helpers
{
    public static class ContextMockHelper
    {
        public static CesiZenDbContext CreateCategoryContextMock(List<Category> categories)
        {
            var dbSetMock = MockDbSetHelper.CreateMockDbSet(categories);
            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(db => db.Categories).Returns(dbSetMock.Object);
            return ctxMock.Object;
        }

        public static CesiZenDbContext CreateUserContextMock(List<User> users)
        {
            var dbSetMock = MockDbSetHelper.CreateMockDbSet(users);
            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(db => db.Users).Returns(dbSetMock.Object);
            return ctxMock.Object;
        }

        public static CesiZenDbContext CreateActivityContext(List<Activity> activities)
        {
            var dbSetMock = MockDbSetHelper.CreateMockDbSet(activities);
            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(db => db.Activities).Returns(dbSetMock.Object);
            return ctxMock.Object;
        }

        public static CesiZenDbContext CreateContextWithArticles(List<Article> articles)
        {
            var mockSet = MockDbSetHelper.CreateMockDbSet(articles);
            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Articles).Returns(mockSet.Object);
            return ctxMock.Object;
        }
    }
}
