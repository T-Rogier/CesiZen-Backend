namespace CesiZen_Backend.Dtos.ActivityDtos
{
    public record ActivityDto(
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
        string Type
    );
}
