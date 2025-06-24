using CesiZen_Backend.Dtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace CesiZen_Backend.Tests.Services.Activities
{
    public class ActivityService_GetActivitiesByCategory_Tests
    {
        [Fact]
        public async Task GivenSomeActivitiesInCategory_ReturnsOnlyThoseDescending()
        {
            // Arrange
            var a1 = BuildActivityWithCategories(1, 100, 200);
            var a2 = BuildActivityWithCategories(2, 200);
            var a3 = BuildActivityWithCategories(3, 300);
            var activities = new List<Activity> { a1, a2, a3 };

            var ctx = ContextMockHelper.CreateActivityContext(activities);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var paging = new PagingRequestDto { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await svc.GetActivitiesByCategoryAsync(categoryId: 200, paging);

            // Assert

            Assert.Equal(2, result.TotalCount);
            Assert.Collection(result.Activities,
                dto1 => Assert.Equal(2, dto1.Id),
                dto2 => Assert.Equal(1, dto2.Id)
            );
        }

        [Fact]
        public async Task GivenNoActivitiesInCategory_ReturnsEmptyAndZeroCount()
        {
            // Arrange
            var activities = new List<Activity> { BuildActivityWithCategories(1, 100) };
            var ctx = ContextMockHelper.CreateActivityContext(activities);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var paging = new PagingRequestDto { PageNumber = 1, PageSize = 5 };

            // Act
            var result = await svc.GetActivitiesByCategoryAsync(categoryId: 999, paging);

            // Assert
            Assert.Empty(result.Activities);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task Pagination_IsAppliedCorrectly()
        {
            // Arrange
            var activities = Enumerable.Range(1, 5)
                                       .Select(i => BuildActivityWithCategories(i, 50))
                                       .ToList();
            var ctx = ContextMockHelper.CreateActivityContext(activities);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());

            var paging = new PagingRequestDto { PageNumber = 2, PageSize = 2 };

            // Act
            var result = await svc.GetActivitiesByCategoryAsync(categoryId: 50, paging);

            // Assert
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Activities.Count());
            Assert.Equal(3, result.Activities.ToArray()[0].Id);
            Assert.Equal(2, result.Activities.ToArray()[1].Id);
        }

        private Activity BuildActivityWithCategories(int id, params int[] categoryIds)
        {
            var user = User.Create($"useruser{id}", $"u{id}@ex.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, id * 10);

            var cats = categoryIds
                .Select(cid =>
                {
                    var c = Category.Create("TestCat", "TestIcon");
                    // setter interne pour Id
                    SetPrivateId(c, cid);
                    return c;
                })
                .ToList();

            var act = Activity.Create(
                title: $"Activity{id}",
                content: "whatever",
                description: "desc",
                thumbnailImageLink: "img",
                estimatedDuration: TimeSpan.FromHours(1),
                createdBy: user,
                categories: cats,
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
