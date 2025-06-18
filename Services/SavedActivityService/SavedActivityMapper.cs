using CesiZen_Backend.Dtos.SavedActivityDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.SavedActivityService
{
    public class SavedActivityMapper
    {
        public static SavedActivityResponseDto ToDto(SavedActivity savedActivity)
        {
            return new SavedActivityResponseDto(
                savedActivity.UserId,
                savedActivity.ActivityId,
                savedActivity.IsFavoris,
                savedActivity.State,
                savedActivity.Progress.ToString()
            );
        }
    }
}
