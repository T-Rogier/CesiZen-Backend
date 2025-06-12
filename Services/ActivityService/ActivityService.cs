using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.ActivityService
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
            User? user = await _dbContext.Users.FindAsync(command.CreatedById);
            if (user is null)
                throw new ArgumentNullException($"Invalid User Id: {command.CreatedById}");

            List<Category> categories = await _dbContext.Categories
                .Where(c => command.Categories.Contains(c.Name))
                .ToListAsync();

            ActivityType type = Enum.Parse<ActivityType>(command.Type);

            Activity activity = Activity.Create(command.Title, command.Content, command.Description, command.ThumbnailImageLink, command.EstimatedDuration, user!, categories, type, command.Activated);

            await _dbContext.Activities.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            return ActivityMapper.ToDto(activity);
        }

        public async Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync()
        {
            return await _dbContext.Activities
                .AsNoTracking()
                .Include(a => a.CreatedBy)
                .Include(a => a.Categories)
                .Select(a => ActivityMapper.ToDto(a))
                .ToListAsync();
        }

        public async Task<ActivityDto?> GetActivityByIdAsync(int id)
        {
            Activity? activity = await _dbContext.Activities
                                   .AsNoTracking()
                                   .Include(a => a.CreatedBy)
                                   .Include(a => a.Categories)
                                   .FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null)
                return null;

            return ActivityMapper.ToDto(activity);
        }

        public async Task UpdateActivityAsync(int id, UpdateActivityDto command)
        {
            List<Category> categories = await _dbContext.Categories
                .Where(c => command.Categories.Contains(c.Name))
                .ToListAsync();

            Activity? activityToUpdate = await _dbContext.Activities.FindAsync(id);
            if (activityToUpdate is null)
                throw new ArgumentNullException($"Invalid Activity Id.");
            activityToUpdate.Update(command.Title, command.Content, command.Description, command.ThumbnailImageLink, command.EstimatedDuration, categories, command.Activated, command.Deleted);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActivityAsync(int id)
        {
            Activity? activityToDelete = await _dbContext.Activities.FindAsync(id);
            if (activityToDelete != null)
            {
                _dbContext.Activities.Remove(activityToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
