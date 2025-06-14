namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record SavedActivityResponseDto(
        int UserId,
        int ActivityId,
        bool IsFavoris,
        string State,
        string Progress
    );
}