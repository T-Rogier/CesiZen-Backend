using CesiZen_Backend.Dtos.ActivityDtos;

namespace CesiZen_Backend.Services
{
    public interface IActivityService
    {
        Task<ActivityDto> CreateActivityAsync(CreateActivityDto command);
        Task<ActivityDto?> GetActivityByIdAsync(int id);
        Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync();
        Task UpdateActivityAsync(int id, UpdateActivityDto command);
        Task DeleteActivityAsync(int id);
    }
}
