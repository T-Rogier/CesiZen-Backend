using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services
{
    public class ActivityService : IActivityService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<ActivityService> _logger;
        public ActivityService(CesiZenDbContext dbContext, ILogger<ActivityService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto command)
        {
            var activity = Activity.Create(command.Title, command.Content, command.Description, command.ThumbnailImageLink);

            await _dbContext.Activities.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            return new ActivityDto(
               activity.Id,
               activity.Title,
               activity.Content,
               activity.Description,
               activity.ThumbnailImageLink
            );
        }

        public async Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync()
        {
            return await _dbContext.Activities
                .AsNoTracking()
                .Select(activity => new ActivityDto(
                    activity.Id,
                    activity.Title,
                    activity.Content,
                    activity.Description,
                    activity.ThumbnailImageLink
                ))
                .ToListAsync();
        }

        public async Task<ActivityDto?> GetActivityByIdAsync(int id)
        {
            var activity = await _dbContext.Activities
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
                return null;

            return new ActivityDto(
                activity.Id,
                activity.Title,
                activity.Content,
                activity.Description,
                activity.ThumbnailImageLink
            );
        }

        public async Task UpdateActivityAsync(int id, UpdateActivityDto command)
        {
            var activityToUpdate = await _dbContext.Activities.FindAsync(id);
            if (activityToUpdate is null)
                throw new ArgumentNullException($"Invalid Activity Id.");
            activityToUpdate.Update(command.Title, command.Content, command.Description, command.ThumbnailImageLink);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActivityAsync(int id)
        {
            var activityToDelete = await _dbContext.Activities.FindAsync(id);
            if (activityToDelete != null)
            {
                _dbContext.Activities.Remove(activityToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
