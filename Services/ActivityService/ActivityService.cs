using CesiZen_Backend.Dtos;
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

        public async Task<FullActivityResponseDto> CreateActivityAsync(CreateActivityRequestDto command)
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

            return ActivityMapper.ToFullDto(activity);
        }

        public async Task<ActivityListResponseDto> GetAllActivitiesAsync()
        {
            List<Activity> activities= await _dbContext.Activities
                .AsNoTracking()
                .Include(a => a.CreatedBy)
                .ToListAsync();
            return ActivityMapper.ToListDto(activities, activities.Count);
        }

        public async Task<ActivityListResponseDto> GetActivitiesByFilterAsync(ActivityFilterRequestDto filter)
        {
            int pageNumber = Math.Max(1, filter.PageNumber);
            int pageSize = Math.Max(1, filter.PageSize);

            IQueryable<Activity> query = _dbContext.Activities
                .Include(a => a.Categories)
                .Include(a => a.CreatedBy)
                .Where(a => !a.Deleted);

            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(a => a.Title.Contains(filter.Title));

            if (filter.StartEstimatedDuration.HasValue)
                query = query.Where(a => a.EstimatedDuration >= filter.StartEstimatedDuration.Value);

            if (filter.EndEstimatedDuration.HasValue)
                query = query.Where(a => a.EstimatedDuration <= filter.EndEstimatedDuration.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Created >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Created <= filter.EndDate.Value);

            if (filter.Activated.HasValue)
                query = query.Where(a => a.Activated == filter.Activated.Value);

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                var categoryName = filter.Category.Trim().ToLower();
                query = query.Where(a =>
                  a.Categories.Any(c => c.Name.ToLower() == categoryName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Type) &&
                Enum.TryParse<ActivityType>(filter.Type, ignoreCase: true, out var parsedType))
            {
                query = query.Where(a => a.Type == parsedType);
            }

            int totalCount = await query.CountAsync();

            List<Activity> activities = await query
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public async Task<ActivityListResponseDto> GetActivitiesByCategoryAsync(int categoryId, PagingRequestDto paging)
        {
            int pageNumber = Math.Max(1, paging.PageNumber);
            int pageSize = Math.Max(1, paging.PageSize);

            int totalCount = await _dbContext.Activities.CountAsync();

            List<Activity> activities = await _dbContext.Activities
                .AsNoTracking()
                .Include(a => a.CreatedBy)
                .Where(a => a.Categories.Any(c => c.Id == categoryId) && !a.Deleted)
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public async Task<ActivityListResponseDto> GetActivitiesByCreatorAsync(int userId, PagingRequestDto paging)
        {
            int pageNumber = Math.Max(1, paging.PageNumber);
            int pageSize = Math.Max(1, paging.PageSize);

            int totalCount = await _dbContext.Activities.CountAsync();

            List<Activity> activities = await _dbContext.Activities
                .AsNoTracking()
                .Include(a => a.CreatedBy)
                .Where(a => a.CreatedById == userId)
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public async Task<FullActivityResponseDto?> GetActivityByIdAsync(int id)
        {
            Activity? activity = await _dbContext.Activities
                                   .AsNoTracking()
                                   .Include(a => a.CreatedBy)
                                   .Include(a => a.Categories)
                                   .FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null)
                return null;

            return ActivityMapper.ToFullDto(activity);
        }

        public async Task UpdateActivityAsync(int id, UpdateActivityRequestDto command)
        {
            List<Category> categories = await _dbContext.Categories
                .Where(c => command.Categories.Contains(c.Name))
                .ToListAsync();

            Activity? activityToUpdate = await _dbContext.Activities.FindAsync(id);
            if (activityToUpdate is null)
                throw new ArgumentNullException($"Invalid Activity Id.");
            activityToUpdate.Update(command.Title, command.Content, command.Description, command.ThumbnailImageLink, command.EstimatedDuration, categories, command.Activated, null);
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
