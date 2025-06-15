using CesiZen_Backend.Dtos.ActivityDtos;

namespace CesiZen_Backend.Services.ActivityService
{
    public interface IActivityService
    {
        Task<FullActivityResponseDto> CreateActivityAsync(CreateActivityRequestDto command);
        Task<FullActivityResponseDto?> GetActivityByIdAsync(int id);
        Task<ActivityListResponseDto> GetAllActivitiesAsync();
        Task<ActivityListResponseDto> GetActivitiesByFilterAsync(ActivityFilterRequestDto filter);
        Task UpdateActivityAsync(int id, UpdateActivityRequestDto command);
        Task DeleteActivityAsync(int id);
    }
}
