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
    public class ActivityService_ParticipateActivity_Tests
    {
        private static CesiZenDbContext CreateContext(
            List<Activity> activities,
            List<Participation> participations)
        {
            var activityQueryable = activities.AsQueryable();
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

            var partQueryable = participations.AsQueryable();
            var partSet = new Mock<DbSet<Participation>>();
            partSet.As<IAsyncEnumerable<Participation>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => new TestAsyncEnumerator<Participation>(partQueryable.GetEnumerator()));
            partSet.As<IQueryable<Participation>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Participation>(partQueryable.Provider));
            partSet.As<IQueryable<Participation>>().Setup(m => m.Expression).Returns(partQueryable.Expression);
            partSet.As<IQueryable<Participation>>().Setup(m => m.ElementType).Returns(partQueryable.ElementType);
            partSet.As<IQueryable<Participation>>().Setup(m => m.GetEnumerator())
                .Returns(() => partQueryable.GetEnumerator());
            partSet.Setup(m => m.AddAsync(It.IsAny<Participation>(), It.IsAny<CancellationToken>()))
                  .Callback<Participation, CancellationToken>((p, _) => participations.Add(p))
                  .Returns((Participation p, CancellationToken ct)
                           => new ValueTask<EntityEntry<Participation>>((EntityEntry<Participation>)null));

            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Activities).Returns(activitySet.Object);
            ctxMock.Setup(c => c.Participations).Returns(partSet.Object);
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            return ctxMock.Object;
        }


        [Fact]
        public async Task ParticipateActivityAsync_ExistingActivity_AddsParticipationAndIncrementsViewCount()
        {
            // Arrange
            var creator = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            SetPrivateId(creator, 10);
            var act = Activity.Create("T", "D", "C", "img", TimeSpan.Zero, creator, new List<Category>(), ActivityType.Writting, false);
            SetPrivateId(act, 42);
            int initialViews = act.ViewCount;

            var activities = new List<Activity> { act };
            var participations = new List<Participation>();
            var ctx = CreateContext(activities, participations);

            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var currentUser = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            SetPrivateId(currentUser, 55);
            var cmd = new ParticipateActivityRequestDto(
                ParticipationDate: DateTime.UtcNow.Date,
                Duration: TimeSpan.FromHours(2)
            );

            // Act
            await service.ParticipateActivityAsync(42, cmd, currentUser);

            // Assert
            // 1) Une Participation a été ajoutée dans la liste
            Assert.Single(participations);
            var p = participations.Single();
            Assert.Equal(42, p.ActivityId);
            Assert.Equal(55, p.UserId);
            Assert.Equal(cmd.ParticipationDate, p.ParticipationDate);
            Assert.Equal(cmd.Duration, p.Duration);

            // 2) Le compteur de vues a été incrémenté
            Assert.Equal(initialViews + 1, act.ViewCount);
        }

        [Fact]
        public async Task ParticipateActivityAsync_InvalidActivity_ThrowsException()
        {
            // Arrange: pas d'activités en base
            var ctx = CreateContext(new List<Activity>(), new List<Participation>());
            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var user = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, 1);
            var cmd = new ParticipateActivityRequestDto(DateTime.Today, TimeSpan.Zero);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(
                () => service.ParticipateActivityAsync(99, cmd, user));
            Assert.Contains("Activity with ID 99 not found", ex.Message);
        }

        static void SetPrivateId<T>(T entity, int id)
        {
            var prop = typeof(T).GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            prop.SetValue(entity, id);
        }
    }
}
