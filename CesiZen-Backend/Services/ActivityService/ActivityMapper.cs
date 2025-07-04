﻿using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ActivityService
{
    public static class ActivityMapper
    {
        public static FullActivityResponseDto ToFullDto(Activity activity, SavedActivity? savedActivity)
        {
            return new FullActivityResponseDto(
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
                activity.Type,
                savedActivity?.IsFavoris,
                savedActivity?.State,
                savedActivity?.Progress.ToString()
            );
        }

        public static SimpleActivityResponseDto ToSimpleDto(Activity activity)
        {
            return new SimpleActivityResponseDto(
                activity.Id,
                activity.Title,
                activity.ThumbnailImageLink,
                activity.EstimatedDuration,
                activity.ViewCount,
                activity.Activated,
                activity.CreatedBy?.Username ?? "Unknown"
            );
        }

        public static ActivityListResponseDto ToListDto(List<Activity> activities, int totalCount, int pageNumber = 1, int pageSize = 10)
        {
            return new ActivityListResponseDto(
                activities.Select(ToSimpleDto),
                pageNumber,
                pageSize,
                totalCount,
                totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0)
            );
        }
    }
}
