using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ActivityService
{
    public interface IActivityService
    {
        Task<FullActivityResponseDto> CreateActivityAsync(CreateActivityRequestDto command, User creator);
        Task<FullActivityResponseDto?> GetActivityByIdAsync(int id, User? currentUser);
        Task<ActivityListResponseDto> GetAllActivitiesAsync();
        Task<ActivityListResponseDto> GetActivitiesByFilterAsync(ActivityFilterRequestDto filter);
        Task<ActivityListResponseDto> GetActivitiesByCategoryAsync(int categoryId, PagingRequestDto paging);
        Task<ActivityListResponseDto> GetActivitiesByCreatorAsync(int creatorId, PagingRequestDto paging);
        Task<ActivityListResponseDto> GetActivitiesByStateAsync(string state, User currentUser, PagingRequestDto paging);
        Task<ActivityListResponseDto> GetFavoritesActivitiesAsync(User currentUser, PagingRequestDto paging);
        Task UpdateActivityAsync(int id, UpdateActivityRequestDto command);
        Task DeleteActivityAsync(int id);
    }
}
