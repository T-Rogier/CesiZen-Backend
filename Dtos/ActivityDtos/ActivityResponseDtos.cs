using CesiZen_Backend.Models;

namespace CesiZen_Backend.Dtos.ActivityDtos
{
    public record FullActivityResponseDto(
        int Id,
        string Title,
        string Description,
        string Content,
        string ThumbnailImageLink,
        TimeSpan EstimatedDuration,
        int ViewCount,
        bool Activated,
        bool Deleted,
        int CreatedById,
        string CreatedBy,
        ICollection<string> Categories,
        ActivityType Type,
        bool? IsFavoris,
        SavedActivityStates? State,
        string? Progress
    );

    public record SimpleActivityResponseDto(
        int Id,
        string Title,
        string ThumbnailImageLink,
        TimeSpan EstimatedDuration,
        int ViewCount,
        bool Activated,
        string CreatedBy
    );

    public record ActivityListResponseDto(
        IEnumerable<SimpleActivityResponseDto> Activities,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages
    );

    public record ActivityTypeListReponseDto(
        IEnumerable<ActivityType> ActivityTypes
    );
}
