using CesiZen_Backend.Models;

namespace CesiZen_Backend.Dtos.SavedActivityDtos
{
    public record SavedActivityResponseDto(
        int UserId,
        int ActivityId,
        bool IsFavoris,
        SavedActivityStates State,
        string Progress
    );
}