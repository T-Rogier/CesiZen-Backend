using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Dtos.SavedActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.SavedActivityService
{
    public class SavedActivityService : ISavedActivityService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<SavedActivityService> _logger;
        public SavedActivityService(CesiZenDbContext dbContext, ILogger<SavedActivityService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<SavedActivityResponseDto> CreateSavedActivityAsync(CreateSavedActivityRequestDto command)
        {
            User? user = await _dbContext.Users.FindAsync(command.UserId);
            if (user == null)
                throw new Exception($"User with ID {command.UserId} not found.");

            Activity? activity = await _dbContext.Activities.FindAsync(command.ActivityId);
            if (activity == null)
                throw new Exception($"Activity with ID {command.ActivityId} not found.");

            SavedActivityStates state = Enum.Parse<SavedActivityStates>(command.State);

            SavedActivity savedActivity = SavedActivity.Create(user, activity, command.IsFavoris, state, new Percentage(command.Progress));

            await _dbContext.SavedActivities.AddAsync(savedActivity);
            await _dbContext.SaveChangesAsync();

            return SavedActivityMapper.ToDto(savedActivity);
        }

        public async Task<SavedActivityResponseDto?> GetSavedActivityByIdsAsync(int userId, int activityId)
        {
            SavedActivity? savedActivity = await _dbContext.SavedActivities
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.UserId == userId && p.ActivityId == activityId);
            if (savedActivity == null)
                return null;

            return SavedActivityMapper.ToDto(savedActivity);
        }

        public async Task<IEnumerable<SavedActivityResponseDto>> GetAllSavedActivitiesAsync()
        {
            return await _dbContext.SavedActivities
                .AsNoTracking()
                .Select(p => SavedActivityMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<SavedActivityResponseDto>> GetSavedActivitiesByUserIdAsync(int userId)
        {
            return await _dbContext.SavedActivities
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => SavedActivityMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<SavedActivityResponseDto>> GetSavedActivitiesByActivityIdAsync(int activityId)
        {
            return await _dbContext.SavedActivities
                .AsNoTracking()
                .Where(p => p.ActivityId == activityId)
                .Select(p => SavedActivityMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task UpdateActivityAsync(int id, UpdateSavedActivityRequestDto command)
        {
            SavedActivity? savedActivityToUpdate = await _dbContext.SavedActivities.FindAsync(id);
            if (savedActivityToUpdate == null)
                throw new Exception($"SavedActivity with ID {id} not found.");

            SavedActivityStates state = Enum.Parse<SavedActivityStates>(command.State);

            savedActivityToUpdate.Update(command.IsFavoris, state, new Percentage(command.Progress));

            _dbContext.SavedActivities.Update(savedActivityToUpdate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSavedActivityAsync(int id)
        {
            SavedActivity? savedActivityToDelete = await _dbContext.SavedActivities.FindAsync(id);
            if (savedActivityToDelete != null)
            {
                _dbContext.SavedActivities.Remove(savedActivityToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
