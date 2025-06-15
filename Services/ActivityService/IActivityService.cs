using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ActivityDtos;

namespace CesiZen_Backend.Services.ActivityService
{
    public interface IActivityService
    {
        Task<FullActivityResponseDto> CreateActivityAsync(CreateActivityRequestDto command);
        Task<FullActivityResponseDto?> GetActivityByIdAsync(int id);
        Task<ActivityListResponseDto> GetAllActivitiesAsync();
        Task<ActivityListResponseDto> GetActivitiesByFilterAsync(ActivityFilterRequestDto filter);
        Task<ActivityListResponseDto> GetActivitiesByCategoryAsync(int categoryId, PagingRequestDto paging);
        Task<ActivityListResponseDto> GetActivitiesByCreatorAsync(int creatorId, PagingRequestDto paging);
        Task UpdateActivityAsync(int id, UpdateActivityRequestDto command);
        Task DeleteActivityAsync(int id);
    }
}
