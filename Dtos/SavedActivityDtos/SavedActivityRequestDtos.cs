namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record CreateSavedActivityRequestDto(
        int UserId,
        int ActivityId,
        bool IsFavoris,
        string State,
        decimal Progress
    );

    public record UpdateSavedActivityRequestDto(
        bool IsFavoris,
        string State,
        decimal Progress
    );
}
