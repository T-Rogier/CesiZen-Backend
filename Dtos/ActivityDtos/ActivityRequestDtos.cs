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
}
