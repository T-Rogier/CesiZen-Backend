namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record SavedActivityDto(
        int UserId,
        int ActivityId,
        bool IsFavoris,
        string State,
        string Progress
    );
}
