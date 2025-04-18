using CesiZen_Backend.Dtos.ParticipationDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.ParticipationService
{
    public class ParticipationService : IParticipationService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<ParticipationService> _logger;
        public ParticipationService(CesiZenDbContext dbContext, ILogger<ParticipationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ParticipationDto> CreateParticipationAsync(CreateParticipationDto command)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId);
            if (user == null)
                throw new Exception($"User with ID {command.UserId} not found.");

            var activity = await _dbContext.Activities.FindAsync(command.ActivityId);
            if (activity == null)
                throw new Exception($"Activity with ID {command.ActivityId} not found.");

            var participation = Participation.Create(user, activity, command.ParticipationDate, command.Duration);

            await _dbContext.Participations.AddAsync(participation);
            await _dbContext.SaveChangesAsync();

            return ParticipationMapper.ToDto(participation);
        }

        public async Task<ParticipationDto?> GetParticipationByIdAsync(int id)
        {
            var participation = await _dbContext.Participations
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.Id == id);
            if (participation == null)
                return null;

            return ParticipationMapper.ToDto(participation);
        }

        public async Task<IEnumerable<ParticipationDto>> GetParticipationsByIdsAsync(int userId, int activityId)
        {
            return await _dbContext.Participations
                .AsNoTracking()
                .Where(p => p.UserId == userId && p.ActivityId == activityId)
                .Select(p => ParticipationMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<ParticipationDto>> GetAllParticipationsAsync()
        {
            return await _dbContext.Participations
                .AsNoTracking()
                .Select(p => ParticipationMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<ParticipationDto>> GetParticipationsByUserIdAsync(int userId)
        {
            return await _dbContext.Participations
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => ParticipationMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<ParticipationDto>> GetParticipationsByActivityIdAsync(int activityId)
        {
            return await _dbContext.Participations
                .AsNoTracking()
                .Where(p => p.ActivityId == activityId)
                .Select(p => ParticipationMapper.ToDto(p))
                .ToListAsync();
        }        

        public async Task DeleteParticipationAsync(int id)
        {
            var participationToDelete = await _dbContext.Participations.FindAsync(id);
            if (participationToDelete != null)
            {
                _dbContext.Participations.Remove(participationToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
