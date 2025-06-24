using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Activities
{
    public class ActivityService_GetActivitiesByFilter_Tests
    {
        [Fact]
        public async Task GetActivitiesByFilterAsync_NoFilter_ReturnsAllDescending()
        {
            // Arrange
            var acts = new List<Activity>
            {
                Builder.BuildActivity(1, "A", TimeSpan.FromHours(1), true, false, ActivityType.Writting),
                Builder.BuildActivity(2, "B", TimeSpan.FromHours(2), true, false, ActivityType.Playlist),
            };
            var ctx = ContextMockHelper.CreateActivityContext(acts);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var filter = new ActivityFilterRequestDto(null, null, null, null, null, null, null, null, PageNumber: 1, PageSize: 10);

            // Act
            var result = await svc.GetActivitiesByFilterAsync(filter);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Activities.Count());

            Assert.Collection(result.Activities,
                dto1 => Assert.Equal(2, dto1.Id),
                dto2 => Assert.Equal(1, dto2.Id)
            );
        }
        [Fact]
        public async Task GetActivitiesByFilterAsync_TitleFilter_IsCaseInsensitive()
        {
            // Arrange
            var acts = new List<Activity>
            {
                Builder.BuildActivity(1, "FooBar", TimeSpan.Zero, true, false, ActivityType.Writting),
                Builder.BuildActivity(2, "Other", TimeSpan.Zero, true, false, ActivityType.Writting),
            };
            var ctx = ContextMockHelper.CreateActivityContext(acts);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var filter = new ActivityFilterRequestDto("foo", null, null, null, null, null, null, null, PageNumber: 1, PageSize: 10);

            // Act
            var result = await svc.GetActivitiesByFilterAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            var dto = Assert.Single(result.Activities);
            Assert.Contains("foo", dto.Title, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetActivitiesByFilterAsync_DurationFilter_WorksAsExpected()
        {
            // Arrange
            var acts = new List<Activity>
            {
                Builder.BuildActivity(1, "T1", TimeSpan.FromHours(1), true, false, ActivityType.Writting),
                Builder.BuildActivity(2, "T2", TimeSpan.FromHours(2), true, false, ActivityType.Writting),
                Builder.BuildActivity(3, "T3", TimeSpan.FromHours(3), true, false, ActivityType.Writting),
            };
            var ctx = ContextMockHelper.CreateActivityContext(acts);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var filter = new ActivityFilterRequestDto(null, TimeSpan.FromHours(1.5), TimeSpan.FromHours(2.5), null, null, null, null, null, PageNumber: 1, PageSize: 10);

            // Act
            var result = await svc.GetActivitiesByFilterAsync(filter);

            Assert.Equal(1, result.TotalCount);
            Assert.Equal(2, result.Activities.First().Id);
        }

        [Fact]
        public async Task GetActivitiesByFilterAsync_CategoryFilter_MatchesAnyCategory()
        {
            // Arrange
            var acts = new List<Activity>
            {
                Builder.BuildActivity(1, "T", TimeSpan.Zero, true, false, ActivityType.Writting, "Alpha"),
                Builder.BuildActivity(2, "T", TimeSpan.Zero, true, false, ActivityType.Writting, "Beta"),
                Builder.BuildActivity(3, "T", TimeSpan.Zero, true, false, ActivityType.Writting, "alpha", "gamma"),
            };
            var ctx = ContextMockHelper.CreateActivityContext(acts);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var filter = new ActivityFilterRequestDto(null, null, null, null, null, null, "alpha", null, PageNumber: 1, PageSize: 10);

            // Act
            var result = await svc.GetActivitiesByFilterAsync(filter);

            Assert.Equal(2, result.TotalCount);
            Assert.Collection(result.Activities,
                dto1 => Assert.Equal(3, dto1.Id),
                dto2 => Assert.Equal(1, dto2.Id)
            );
        }

        [Fact]
        public async Task GetActivitiesByFilterAsync_TypeFilter_Works()
        {
            // Arrange
            var acts = new List<Activity>
            {
                Builder.BuildActivity(1, "T", TimeSpan.Zero, true, false, ActivityType.Writting),
                Builder.BuildActivity(2, "T", TimeSpan.Zero, true, false, ActivityType.Playlist),
            };
            var ctx = ContextMockHelper.CreateActivityContext(acts);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var filter = new ActivityFilterRequestDto(null, null, null, null, null, null, null, ActivityType.Playlist, PageNumber: 1, PageSize: 10);

            // Act
            var result = await svc.GetActivitiesByFilterAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(2, result.Activities.First().Id);
        }

        [Fact]
        public async Task GetActivitiesByFilterAsync_Paging_RespectsPageNumberAndSize()
        {
            // Arrange
            var acts = Enumerable.Range(1, 5)
                                 .Select(i => Builder.BuildActivity(i, $"T{i}", TimeSpan.Zero, true, false, ActivityType.Writting))
                                 .ToList();

            var ctx = ContextMockHelper.CreateActivityContext(acts);
            var svc = new ActivityService(ctx, Mock.Of<ILogger<ActivityService>>());
            var filter = new ActivityFilterRequestDto(null, null, null, null, null, null, null, null, PageNumber: 2, PageSize: 2);

            // Act
            var result = await svc.GetActivitiesByFilterAsync(filter);

            // Assert
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Activities.Count());
            Assert.Equal(3, result.Activities.ToArray()[0].Id);
            Assert.Equal(2, result.Activities.ToArray()[1].Id);
        }
    }
}
