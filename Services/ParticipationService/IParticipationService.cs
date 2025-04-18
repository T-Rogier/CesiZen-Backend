using CesiZen_Backend.Dtos.ParticipationDtos;

namespace CesiZen_Backend.Services.ParticipationService
{
    public interface IParticipationService
    {
        Task<ParticipationDto> CreateParticipationAsync(CreateParticipationDto command);
        Task<ParticipationDto?> GetParticipationByIdAsync(int id);
        Task<IEnumerable<ParticipationDto>> GetAllParticipationsAsync();
        Task<IEnumerable<ParticipationDto>> GetParticipationsByIdsAsync(int activityId, int userId);
        Task<IEnumerable<ParticipationDto>> GetParticipationsByUserIdAsync(int userId);
        Task<IEnumerable<ParticipationDto>> GetParticipationsByActivityIdAsync(int activityId);
        Task DeleteParticipationAsync(int id);
    }
}
