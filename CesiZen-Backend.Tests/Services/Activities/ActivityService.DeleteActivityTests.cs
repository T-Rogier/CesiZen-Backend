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
    public class ActivityService_DeleteActivity_Tests
    {
        [Fact]
        public async Task DeleteActivityAsync_ExistingActivity_MarksDeleted_UpdatesAndSaves()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            int activityId;
            DateTimeOffset beforeDelete;

            using (var seedCtx = new CesiZenDbContext(options))
            {
                var act = Activity.Create(
                    title: "T", content: "C", description: "D", thumbnailImageLink: "img",
                    estimatedDuration: TimeSpan.Zero,
                    createdBy: User.Create("username", "email@example.com", "pwdpwd", UserRole.User),
                    categories: [],
                    type: ActivityType.Writting,
                    activated: true
                );
                seedCtx.Activities.Add(act);
                seedCtx.SaveChanges();

                activityId = act.Id;
                beforeDelete = act.Updated;
            }

            // Act
            using (var ctx = new CesiZenDbContext(options))
            {
                var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
                await svc.DeleteActivityAsync(activityId);
            }

            // Assert
            using var assertCtx = new CesiZenDbContext(options);
            var updated = await assertCtx.Activities.FirstAsync(a => a.Id == activityId);

            Assert.True(updated.Deleted);

            Assert.NotEqual(beforeDelete, updated.Updated);

            Assert.True(updated.Updated > DateTimeOffset.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task DeleteActivityAsync_InvalidId_DoesNothing()
        {
            // Arrange
            var actSetMock = new Mock<DbSet<Activity>>();
            actSetMock
                .Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns<object[], CancellationToken>((ids, ct)
                    => new ValueTask<Activity?>((Activity?)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Activities).Returns(actSetMock.Object);
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            var service = new ActivityService(ctxMock.Object, Mock.Of<ILogger<ActivityService>>());

            // Act
            var ex = await Record.ExceptionAsync(() => service.DeleteActivityAsync(999));

            // Assert
            Assert.Null(ex);
            Mock.Get(ctxMock.Object)
                .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
