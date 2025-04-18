namespace CesiZen_Backend.Dtos.ActivityDtos
{
    public record UpdateActivityDto(
        string Title,
        string Description,
        string Content,
        string ThumbnailImageLink,
        TimeSpan EstimatedDuration,
        int ViewCount,
        bool Activated,
        bool Deleted,
        ICollection<string> Categories
    );
}
