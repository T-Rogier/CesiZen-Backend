using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace CesiZen_Backend.Tests.Services.Activities
{
    public class ActivityService_UpdateActivity_Tests
    {
        [Fact]
        public async Task UpdateActivityAsync_ExistingActivity_UpdatesPropertiesCategoriesAndSaves()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            int activityId;
            // Seed
            using (var seed = new CesiZenDbContext(options))
            {
                var catA = Category.Create("CatA", "IconA");
                var catB = Category.Create("CatB", "IconB");
                seed.Categories.AddRange(catA, catB);

                var act = Activity.Create(
                    title: "OldTitle",
                    content: "OldContent",
                    description: "OldDesc",
                    thumbnailImageLink: "OldImg",
                    estimatedDuration: TimeSpan.FromHours(1),
                    createdBy: User.Create("username", "email@example.com", "pwdpwd", UserRole.User),
                    categories: [],
                    type: ActivityType.Writting,
                    activated: false
                );
                seed.Activities.Add(act);
                seed.SaveChanges();
                activityId = act.Id;
            }

            // Act
            using (var ctx = new CesiZenDbContext(options))
            {
                var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
                var cmd = new UpdateActivityRequestDto(
                    Title: "NewTitle",
                    Content: "NewContent",
                    Description: "NewDesc",
                    ThumbnailImageLink: "NewImg",
                    EstimatedDuration: TimeSpan.FromHours(2),
                    Categories: ["CatA", "CatB"],
                    Activated: true
                );

                await svc.UpdateActivityAsync(activityId, cmd);
            }

            // Assert
            using var assertCtx = new CesiZenDbContext(options);
            var updated = await assertCtx.Activities
                .Include(a => a.Categories)
                .FirstAsync(a => a.Id == activityId);

            Assert.Equal("NewTitle", updated.Title);
            Assert.Equal("NewContent", updated.Content);
            Assert.Equal("NewDesc", updated.Description);
            Assert.Equal("NewImg", updated.ThumbnailImageLink);
            Assert.Equal(TimeSpan.FromHours(2), updated.EstimatedDuration);
            Assert.True(updated.Activated);

            var catNames = updated.Categories
                .Select(c => c.Name)
                .OrderBy(n => n)
                .ToList();
            Assert.Equal(["CatA", "CatB"], catNames);
        }

        [Fact]
        public async Task UpdateActivityAsync_InvalidId_ThrowsArgumentNullException()
        {
            // Arrange
            var emptyCats = new List<Category>().AsQueryable();
            var catSetMock = new Mock<DbSet<Category>>();
            catSetMock.As<IAsyncEnumerable<Category>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => new TestAsyncEnumerator<Category>(emptyCats.GetEnumerator()));
            catSetMock.As<IQueryable<Category>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Category>(emptyCats.Provider));
            catSetMock.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(emptyCats.Expression);
            catSetMock.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(emptyCats.ElementType);
            catSetMock.As<IQueryable<Category>>().Setup(m => m.GetEnumerator())
                .Returns(() => emptyCats.GetEnumerator());

            var actSetMock = new Mock<DbSet<Activity>>();
            actSetMock
                .Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids => new ValueTask<Activity?>((Activity?)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Categories).Returns(catSetMock.Object);
            ctxMock.Setup(c => c.Activities).Returns(actSetMock.Object);

            var svc = new ActivityService(ctxMock.Object, Mock.Of<ILogger<ActivityService>>());
            var cmd = new UpdateActivityRequestDto(
                Title: "X",
                Content: "X",
                Description: "X",
                ThumbnailImageLink: "X",
                EstimatedDuration: TimeSpan.Zero,
                Categories: Array.Empty<string>(),
                Activated: false
            );

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => svc.UpdateActivityAsync(999, cmd)
            );
        }
    }
}
