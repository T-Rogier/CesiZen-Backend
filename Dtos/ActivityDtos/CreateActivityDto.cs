namespace CesiZen_Backend.Dtos.ActivityDtos
{
    public record CreateActivityDto(string Title, string Description, string Content, string ThumbnailImageLink, TimeSpan EstimatedDuration, bool Activated, int CreatedById, ICollection<string> Categories, string Type);
}
