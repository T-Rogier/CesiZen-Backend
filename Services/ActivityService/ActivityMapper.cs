using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ActivityService
{
    public static class ActivityMapper
    {
        public static ActivityDto ToDto(Activity activity)
        {
            return new ActivityDto(
                activity.Id,
                activity.Title,
                activity.Description,
                activity.Content,
                activity.ThumbnailImageLink,
                activity.EstimatedDuration,
                activity.ViewCount,
                activity.Activated,
                activity.Deleted,
                activity.CreatedById,
                activity.CreatedBy?.Username ?? "Unknown",
                activity.Categories.Select(c => c.Name).ToList(),
                activity.Type.GetDisplayName()
            );
        }
    }
}
