namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record UpdateSavedActivityDto(int UserId, int ActivityId, bool IsFavoris, string State, string Progress);
}
