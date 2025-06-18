using CesiZen_Backend.Models;

namespace CesiZen_Backend.Dtos.ActivityDtos
{
    public record CreateActivityRequestDto(
        string Title,
        string Description,
        string Content,
        string ThumbnailImageLink,
        TimeSpan EstimatedDuration,
        bool Activated,
        ICollection<string> Categories,
        ActivityType Type
    );

    public record UpdateActivityRequestDto(
        string Title,
        string Description,
        string Content,
        string ThumbnailImageLink,
        TimeSpan EstimatedDuration,
        bool Activated,
        ICollection<string> Categories
    );

    public record ActivityFilterRequestDto(
        string? Title,
        TimeSpan? StartEstimatedDuration,
        TimeSpan? EndEstimatedDuration,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        bool? Activated,
        string? Category,
        ActivityType? Type,
        int PageNumber = 1,
        int PageSize = 10
    );

    public record ActivityByStateRequestDto(
        SavedActivityStates State,
        int PageNumber = 1,
        int PageSize = 10
    );

    public record SaveActivityRequestDto(
        bool IsFavoris,
        SavedActivityStates State,
        decimal Progress
    );

    public record ParticipateActivityRequestDto(
        DateTime ParticipationDate,
        TimeSpan Duration
    );
}
