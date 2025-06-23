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

        public async Task<FullActivityResponseDto> CreateActivityAsync(CreateActivityRequestDto command, User creator)
        {
            List<Category> categories = await _dbContext.Categories
                .Where(c => command.Categories.Contains(c.Name))
                .ToListAsync();

            Activity activity = Activity.Create(command.Title, command.Content, command.Description, command.ThumbnailImageLink, command.EstimatedDuration, creator, categories, command.Type, command.Activated);

            await _dbContext.Activities.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            return ActivityMapper.ToFullDto(activity, null);
        }

        public async Task<FullActivityResponseDto> SaveActivityAsync(int activityId, SaveActivityRequestDto command, User currentUser)
        {
            Activity? activity = await _dbContext.Activities.FindAsync(activityId);
            if (activity == null)
                throw new Exception($"Activity with ID {activityId} not found.");

            SavedActivity? savedActivity = await _dbContext.SavedActivities
                .AsNoTracking()
                .FirstOrDefaultAsync(sa => sa.ActivityId == activityId && sa.UserId == currentUser.Id);
            if (savedActivity == null)
            {
                savedActivity = SavedActivity.Create(currentUser, activity, command.IsFavoris, command.State, new Percentage(command.Progress));
                await _dbContext.SavedActivities.AddAsync(savedActivity);
            }
            else             {
                savedActivity.Update(command.IsFavoris, command.State, new Percentage(command.Progress));
                _dbContext.SavedActivities.Update(savedActivity);
            }

            await _dbContext.SaveChangesAsync();

            return ActivityMapper.ToFullDto(activity, savedActivity);
        }

        public async Task ParticipateActivityAsync(int activityId, ParticipateActivityRequestDto command, User currentUser)
        {
            Activity? activity = await _dbContext.Activities.FindAsync(activityId);
            if (activity == null)
                throw new Exception($"Activity with ID {activityId} not found.");

            Participation participation = Participation.Create(currentUser, activity, command.ParticipationDate, command.Duration);
            activity.AddViewCount();

            await _dbContext.Participations.AddAsync(participation);
            await _dbContext.SaveChangesAsync();
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
                query = query.Where(a => a.Title.ToLower().Contains(filter.Title.ToLower()));

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
                  a.Categories.Any(c => c.Name.ToLower().Equals(categoryName)));
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(a => a.Type == filter.Type.Value);
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

            int totalCount = await _dbContext.Activities.Where(a => a.Categories.Any(c => c.Id == categoryId) && !a.Deleted).CountAsync();

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

            int totalCount = await _dbContext.Activities.Where(a => a.CreatedById == userId && !a.Deleted).CountAsync();

            List<Activity> activities = await _dbContext.Activities
                .AsNoTracking()
                .Include(a => a.CreatedBy)
                .Where(a => a.CreatedById == userId && !a.Deleted)
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public async Task<ActivityListResponseDto> GetActivitiesByStateAsync(User currentUser, ActivityByStateRequestDto filter)
        {
            int pageNumber = Math.Max(1, filter.PageNumber);
            int pageSize = Math.Max(1, filter.PageSize);

            IQueryable<Activity> activitiesQuery = _dbContext.SavedActivities
                .AsNoTracking()
                .Include(sa => sa.Activity)
                .Where(sa => sa.UserId == currentUser.Id)
                .Where(sa => sa.State == filter.State)
                .Select(sa => sa.Activity);

            int totalCount = await activitiesQuery.CountAsync();

            List<Activity> activities = await activitiesQuery
                .OrderByDescending(a => a.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public async Task<ActivityListResponseDto> GetFavoritesActivitiesAsync(User currentUser, PagingRequestDto paging)
        {
            int pageNumber = Math.Max(1, paging.PageNumber);
            int pageSize = Math.Max(1, paging.PageSize);

            IQueryable<Activity> activitiesQuery = _dbContext.SavedActivities
                .AsNoTracking()
                .Include(sa => sa.Activity)
                .Where(sa => sa.UserId == currentUser.Id)
                .Where(sa => sa.IsFavoris)
                .Select(sa => sa.Activity);

            int totalCount = await activitiesQuery.CountAsync();

            List<Activity> activities = await activitiesQuery
                .OrderByDescending(a => a.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public async Task<ActivityListResponseDto> GetSavedActivitiesAsync(User currentUser, PagingRequestDto paging)
        {
            int pageNumber = Math.Max(1, paging.PageNumber);
            int pageSize = Math.Max(1, paging.PageSize);

            IQueryable<Activity> activitiesQuery = _dbContext.SavedActivities
                .AsNoTracking()
                .Include(sa => sa.Activity)
                .Where(sa => sa.UserId == currentUser.Id)
                .Select(sa => sa.Activity);

            int totalCount = await activitiesQuery.CountAsync();

            List<Activity> activities = await activitiesQuery
                .OrderByDescending(a => a.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ActivityMapper.ToListDto(activities, totalCount, pageNumber, pageSize);
        }

        public Task<ActivityTypeListReponseDto> GetActivityTypesAsync()
        {
            IEnumerable<ActivityType> activityTypes = Enum.GetValues<ActivityType>();
            return Task.FromResult(new ActivityTypeListReponseDto(activityTypes));
        }

        public async Task<FullActivityResponseDto?> GetActivityByIdAsync(int id, User? currentUser)
        {
            Activity? activity = await _dbContext.Activities
                                   .AsNoTracking()
                                   .Include(a => a.CreatedBy)
                                   .Include(a => a.Categories)
                                   .FirstOrDefaultAsync(a => a.Id == id);

            SavedActivity? savedActivity = null;
            if (currentUser != null)
            {
                savedActivity = await _dbContext.SavedActivities
                   .AsNoTracking()
                   .FirstOrDefaultAsync(sa => sa.ActivityId == id && sa.UserId == currentUser.Id);
            }
            
            if (activity == null)
                return null;

            return ActivityMapper.ToFullDto(activity, savedActivity);
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
                activityToDelete.Delete();
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
