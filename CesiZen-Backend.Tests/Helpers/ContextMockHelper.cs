using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CesiZen_Backend.Tests.Helpers
{
    public static class ContextMockHelper
    {
        public static CesiZenDbContext CreateCategoryContextMock(List<Category> data)
        {
            var dbSetMock = MockDbSetHelper.CreateMockDbSet(data);
            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(db => db.Categories).Returns(dbSetMock.Object);
            return ctxMock.Object;
        }

        public static CesiZenDbContext CreateUserContextMock(IEnumerable<User> initialUsers)
        {
            var queryable = initialUsers.AsQueryable();
            var dbSetMock = new Mock<DbSet<User>>();

            // Async LINQ support
            dbSetMock.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => new TestAsyncEnumerator<User>(queryable.GetEnumerator()));
            dbSetMock.As<IQueryable<User>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(queryable.Provider));
            dbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(ctx => ctx.Users).Returns(dbSetMock.Object);
            return ctxMock.Object;
        }

        public static CesiZenDbContext CreateActivityContext(List<Activity> activities)
        {
            // IQueryable sync et async
            var queryable = activities.AsQueryable();

            var mockSet = new Mock<DbSet<Activity>>();
            mockSet.As<IAsyncEnumerable<Activity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => new TestAsyncEnumerator<Activity>(queryable.GetEnumerator()));
            mockSet.As<IQueryable<Activity>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Activity>(queryable.Provider));
            mockSet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Activities).Returns(mockSet.Object);
            return ctxMock.Object;
        }
    }
}
