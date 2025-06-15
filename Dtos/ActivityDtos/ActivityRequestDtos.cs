namespace CesiZen_Backend.Dtos.ActivityDtos
{
    public record CreateActivityRequestDto(
        string Title,
        string Description,
        string Content,
        string ThumbnailImageLink,
        TimeSpan EstimatedDuration,
        bool Activated,
        int CreatedById,
        ICollection<string> Categories,
        string Type
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
        DateTime? StartDate,
        DateTime? EndDate,
        bool? Activated,
        string? Category,
        string? Type,
        int PageNumber = 1,
        int PageSize = 10
    );
}
