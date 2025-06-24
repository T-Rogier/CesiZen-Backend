using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Activities
{
    public class ActivityService_GetAllActivities_Tests
    {
        [Fact]
        public async Task GetAllActivitiesAsync_GivenSomeActivities_ReturnsDtosEtCount()
        {
            // Arrange
            var user1 = User.Create("alice", "alice@example.com", "pwdpwd", UserRole.User);
            var user2 = User.Create("bob", "bob@example.com", "pwdpwd", UserRole.User);

            var act1 = Activity.Create("T1", "C1", "D1", "img1", TimeSpan.Zero, user1, new List<Category>(), ActivityType.Writting, false);
            var act2 = Activity.Create("T2", "C2", "D2", "img2", TimeSpan.Zero, user2, new List<Category>(), ActivityType.Playlist, false);

            var ctx = ContextMockHelper.CreateActivityContext(new List<Activity> { act1, act2 });
            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());

            // Act
            var result = await service.GetAllActivitiesAsync();

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Activities.Count());

            Assert.Collection(result.Activities,
                item => Assert.Equal("T1", item.Title),
                item => Assert.Equal("T2", item.Title)
            );
        }

        [Fact]
        public async Task GetAllActivitiesAsync_GivenNoActivities_ReturnsEmptyAndZeroCount()
        {
            // Arrange
            var ctx = ContextMockHelper.CreateActivityContext(new List<Activity>());
            var service = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());

            // Act
            var result = await service.GetAllActivitiesAsync();

            // Assert
            Assert.Empty(result.Activities);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
