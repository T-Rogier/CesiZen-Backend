namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record SavedActivityDto(int Id, int UserId, int ActivityId, bool IsFavoris, string State, string Progress);
}
