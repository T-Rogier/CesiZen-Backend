namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record CreateSavedActivityDto(int UserId, int ActivityId, bool IsFavoris, string State, string Progress);
}
