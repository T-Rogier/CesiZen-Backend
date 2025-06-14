using CesiZen_Backend.Dtos.SavedActivityDtos;

namespace CesiZen_Backend.Services.SavedActivityService
{
    public interface ISavedActivityService
    {
        Task<SavedActivityResponseDto> CreateSavedActivityAsync(CreateSavedActivityRequestDto command);
        Task<SavedActivityResponseDto?> GetSavedActivityByIdsAsync(int activityId, int userId);
        Task<IEnumerable<SavedActivityResponseDto>> GetAllSavedActivitiesAsync();
        Task<IEnumerable<SavedActivityResponseDto>> GetSavedActivitiesByUserIdAsync(int userId);
        Task<IEnumerable<SavedActivityResponseDto>> GetSavedActivitiesByActivityIdAsync(int activityId);
        Task DeleteSavedActivityAsync(int id);
    }
}
