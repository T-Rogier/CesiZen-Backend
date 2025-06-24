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
    public class ActivityService_SaveActivity_Tests
    {
        private static CesiZenDbContext CreateContext(
            List<Activity> activities,
            List<SavedActivity> savedActivities)
        {
            var activityQueryable = activities.AsQueryable();
            var savedQueryable = savedActivities.AsQueryable();

            var activitySet = new Mock<DbSet<Activity>>();
            activitySet.As<IAsyncEnumerable<Activity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => new TestAsyncEnumerator<Activity>(activityQueryable.GetEnumerator()));
            activitySet.As<IQueryable<Activity>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Activity>(activityQueryable.Provider));
            activitySet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(activityQueryable.Expression);
            activitySet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(activityQueryable.ElementType);
            activitySet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator())
                .Returns(() => activityQueryable.GetEnumerator());
            activitySet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids => new ValueTask<Activity?>(activities.SingleOrDefault(a => a.Id == (int)ids[0])));

            var savedSet = new Mock<DbSet<SavedActivity>>();
            savedSet.As<IAsyncEnumerable<SavedActivity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => new TestAsyncEnumerator<SavedActivity>(savedQueryable.GetEnumerator()));
            savedSet.As<IQueryable<SavedActivity>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<SavedActivity>(savedQueryable.Provider));
            savedSet.As<IQueryable<SavedActivity>>().Setup(m => m.Expression).Returns(savedQueryable.Expression);
            savedSet.As<IQueryable<SavedActivity>>().Setup(m => m.ElementType).Returns(savedQueryable.ElementType);
            savedSet.As<IQueryable<SavedActivity>>().Setup(m => m.GetEnumerator())
                .Returns(() => savedQueryable.GetEnumerator());
            savedSet.Setup(m => m.AddAsync(It.IsAny<SavedActivity>(), It.IsAny<CancellationToken>()))
                    .Callback<SavedActivity, CancellationToken>((sa, _) => savedActivities.Add(sa))
                    .Returns((SavedActivity sa, CancellationToken ct) => new ValueTask<EntityEntry<SavedActivity>>((EntityEntry<SavedActivity>)null));
            savedSet.Setup(m => m.Update(It.IsAny<SavedActivity>()))
                    .Callback<SavedActivity>(sa =>
                    {
                        var idx = savedActivities.FindIndex(x =>
                            x.ActivityId == sa.ActivityId &&
                            x.UserId == sa.UserId);
                        if (idx >= 0) savedActivities[idx] = sa;
                    });

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Activities).Returns(activitySet.Object);
            ctxMock.Setup(c => c.SavedActivities).Returns(savedSet.Object);
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
            return ctxMock.Object;
        }

        [Fact]
        public async Task SaveActivityAsync_NewSave_AddsSavedActivity()
        {
            // Arrange
            var creator = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            var act = Activity.Create("T", "D", "C", "img", TimeSpan.Zero, creator, new List<Category>(), ActivityType.Writting, false);
            SetPrivateId(act, 1);
            var activities = new List<Activity> { act };
            var saved = new List<SavedActivity>();
            var ctx = CreateContext(activities, saved);
            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var user = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, 10);
            var cmd = new SaveActivityRequestDto(IsFavoris: true, State: SavedActivityStates.InProgress, Progress: 0);

            // Act
            var result = await service.SaveActivityAsync(1, cmd, user);

            // Assert new saved
            Assert.Single(saved);
            var sa = saved.Single();
            Assert.Equal(1, sa.ActivityId);
            Assert.Equal(10, sa.UserId);
            Assert.True(sa.IsFavoris);
            Assert.Equal(SavedActivityStates.InProgress, sa.State);
            Assert.Equal(0, sa.Progress.Value);
            // DTO result links activity and saved
            Assert.Equal(act.Id, result.Id);
            Assert.Equal(sa.IsFavoris, result.IsFavoris);
        }

        [Fact]
        public async Task SaveActivityAsync_ExistingSave_UpdatesSavedActivity()
        {
            // Arrange existing saved
            var creator = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            var act = Activity.Create("T", "D", "C", "img", TimeSpan.Zero, creator, new List<Category>(), ActivityType.Writting, false);
            SetPrivateId(act, 1);
            SetPrivateId(creator, 10);
            var save = SavedActivity.Create(creator, act, false, SavedActivityStates.NoProgress, new Percentage(0.1m));

            var activities = new List<Activity> { act };
            var saved = new List<SavedActivity> { save };
            var ctx = CreateContext(activities, saved);
            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var user = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, 10);
            var cmd = new SaveActivityRequestDto(IsFavoris: false, State: SavedActivityStates.Completed, Progress: 0.1m);

            // Act
            var result = await service.SaveActivityAsync(1, cmd, user);

            // Assert updated
            Assert.Single(saved);
            var sa = saved.Single();
            Assert.Equal(1, sa.ActivityId);
            Assert.Equal(10, sa.UserId);
            Assert.False(sa.IsFavoris);
            Assert.Equal(SavedActivityStates.Completed, sa.State);
            Assert.Equal(0.1m, sa.Progress.Value);
            Assert.Equal(act.Id, result.Id);
            Assert.Equal(sa.State, result.State);
        }

        [Fact]
        public async Task SaveActivityAsync_InvalidActivity_ThrowsException()
        {
            // Arrange empty activities
            var ctx = CreateContext(new List<Activity>(), new List<SavedActivity>());
            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var user = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, 10);
            var cmd = new SaveActivityRequestDto(true, SavedActivityStates.NoProgress, 0);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.SaveActivityAsync(99, cmd, user));
            Assert.Contains("Activity with ID 99 not found", ex.Message);
        }

        static void SetPrivateId<T>(T entity, int id)
        {
            var prop = typeof(T).GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            prop.SetValue(entity, id);
        }
    }
}
