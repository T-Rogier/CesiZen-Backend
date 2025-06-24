using CesiZen_Backend.Dtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace CesiZen_Backend.Tests.Services.Activities
{
    public class ActivityService_GetActivitiesByCreator_Tests
    {
        [Fact]
        public async Task GivenSomeActivitiesInCategory_ReturnsOnlyThoseDescending()
        {
            // Arrange
            var a1 = BuildActivityWithCreator(1, 10);
            var a2 = BuildActivityWithCreator(2, 20);
            var a3 = BuildActivityWithCreator(3, 20);
            var activities = new List<Activity> { a1, a2, a3 };

            var ctx = ContextMockHelper.CreateActivityContext(activities);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var paging = new PagingRequestDto { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await svc.GetActivitiesByCreatorAsync(userId: 20, paging);

            // Assert

            Assert.Equal(2, result.TotalCount);
            Assert.Collection(result.Activities,
                dto1 => Assert.Equal(3, dto1.Id),
                dto2 => Assert.Equal(2, dto2.Id)
            );
        }

        [Fact]
        public async Task GivenNoActivitiesInCategory_ReturnsEmptyAndZeroCount()
        {
            // Arrange
            var activities = new List<Activity> { BuildActivityWithCreator(1, 10) };
            var ctx = ContextMockHelper.CreateActivityContext(activities);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var paging = new PagingRequestDto { PageNumber = 1, PageSize = 5 };

            // Act
            var result = await svc.GetActivitiesByCreatorAsync(userId: 999, paging);

            // Assert
            Assert.Empty(result.Activities);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task Pagination_IsAppliedCorrectly()
        {
            var activities = Enumerable.Range(1, 5)
                                       .Select(i => BuildActivityWithCreator(i, 50))
                                       .ToList();
            var ctx = ContextMockHelper.CreateActivityContext(activities);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());

            var paging = new PagingRequestDto { PageNumber = 2, PageSize = 2 };

            // Act
            var result = await svc.GetActivitiesByCreatorAsync(userId: 50, paging);

            // Assert
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Activities.Count());
            Assert.Equal(3, result.Activities.ToArray()[0].Id);
            Assert.Equal(2, result.Activities.ToArray()[1].Id);
        }

        private Activity BuildActivityWithCreator(int id, int userId)
        {
            var user = User.Create($"useruser{id}", $"u{id}@ex.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, userId);

            var act = Activity.Create(
                title: $"Activity{id}",
                content: "whatever",
                description: "desc",
                thumbnailImageLink: "img",
                estimatedDuration: TimeSpan.FromHours(1),
                createdBy: user,
                categories: new List<Category>(),
                type: ActivityType.Writting,
                activated: true
            );
            SetPrivateId(act, id);
            return act;
        }

        static void SetPrivateId<T>(T entity, int id)
        {
            var prop = typeof(T).GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            prop.SetValue(entity, id);
        }
    }
}
