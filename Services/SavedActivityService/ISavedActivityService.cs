using CesiZen_Backend.Dtos.SavedActivityDtos;

namespace CesiZen_Backend.Services.SavedActivityService
{
    public interface ISavedActivityService
    {
        Task<SavedActivityDto> CreateSavedActivityAsync(CreateSavedActivityDto command);
        Task<SavedActivityDto?> GetSavedActivityByIdsAsync(int activityId, int userId);
        Task<IEnumerable<SavedActivityDto>> GetAllSavedActivitiesAsync();
        Task<IEnumerable<SavedActivityDto>> GetSavedActivitiesByUserIdAsync(int userId);
        Task<IEnumerable<SavedActivityDto>> GetSavedActivitiesByActivityIdAsync(int activityId);
        Task DeleteSavedActivityAsync(int id);
    }
}
