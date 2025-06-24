using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen_Backend.Tests.Services.Activities
{
    public class ActivityService_CreateActivity_Tests
    {
        [Fact]
        public async Task CreateActivityAsync_ShouldAddActivityAndReturnFullDto()
        {
            // Arrange
            var creator = User.Create("creatorUser", "creator@example.com", "pwdpwd", UserRole.User);
            var command = new CreateActivityRequestDto(
                Title: "Activity Title",
                Content: "Activity Content",
                Description: "Activity Description",
                ThumbnailImageLink: "thumb.png",
                EstimatedDuration: TimeSpan.FromHours(1),
                Categories: new List<string> { "Cat1", "Cat2" },
                Type: ActivityType.Writting,
                Activated: true
            );

            // Prepare categories DbSet
            var existingCategories = new List<Category>
            {
                Category.Create("Cat1", "icon1"),
                Category.Create("Cat2", "icon2"),
                Category.Create("Other", "iconX")
            };
            var categorySetMock = MockDbSetHelper.CreateMockDbSet(existingCategories);

            // Capture added Activity
            var addedActivities = new List<Activity>();
            var activitySetMock = new Mock<DbSet<Activity>>();
            activitySetMock
               .Setup(m => m.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
               .Callback<Activity, CancellationToken>((a, _) => addedActivities.Add(a))
               .Returns((Activity a, CancellationToken ct)
                   => new ValueTask<EntityEntry<Activity>>((EntityEntry<Activity>)null));

            // Mock DbContext
            var options = new DbContextOptionsBuilder<CesiZenDbContext>().Options;
            var ctxMock = new Mock<CesiZenDbContext>(options) { CallBase = false };
            ctxMock.Setup(c => c.Categories).Returns(categorySetMock.Object);
            ctxMock.Setup(c => c.Activities).Returns(activitySetMock.Object);
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            var logger = Mock.Of<ILogger<ActivityService>>();
            var service = new ActivityService(ctxMock.Object, logger);

            // Act
            var result = await service.CreateActivityAsync(command, creator);

            // Assert: interactions
            ctxMock.Verify(c => c.Categories, Times.Once);
            activitySetMock.Verify(m => m.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()), Times.Once);
            ctxMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Assert: captured activity
            Assert.Single(addedActivities);
            var added = addedActivities.Single();
            Assert.Equal(command.Title, added.Title);
            Assert.Equal(command.Content, added.Content);
            Assert.Equal(command.Description, added.Description);
            Assert.Equal(command.ThumbnailImageLink, added.ThumbnailImageLink);
            Assert.Equal(command.EstimatedDuration, added.EstimatedDuration);
            Assert.Equal(creator, added.CreatedBy);
            Assert.Equal(command.Type, added.Type);
            Assert.Equal(command.Activated, added.Activated);
            Assert.Collection(added.Categories.OrderBy(c => c.Name),
                c => Assert.Equal("Cat1", c.Name),
                c => Assert.Equal("Cat2", c.Name)
            );

            // Assert: DTO
            Assert.Equal(added.Id, result.Id);
            Assert.Equal(added.Title, result.Title);
            Assert.Equal(added.Content, result.Content);
            Assert.Equal(added.Categories.Count, result.Categories.Count);
        }
    }
}
