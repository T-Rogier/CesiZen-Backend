using CesiZen_Backend.Dtos.SavedActivityDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.SavedActivityService
{
    public class SavedActivityMapper
    {
        public static SavedActivityDto ToDto(SavedActivity savedActivity)
        {
            return new SavedActivityDto(
                savedActivity.UserId,
                savedActivity.ActivityId,
                savedActivity.IsFavoris,
                savedActivity.State.ToString(),
                savedActivity.Progress.ToString()
            );
        }
    }
}
