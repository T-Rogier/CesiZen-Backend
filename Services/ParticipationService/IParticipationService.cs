using CesiZen_Backend.Dtos.ParticipationDtos;

namespace CesiZen_Backend.Services.ParticipationService
{
    public interface IParticipationService
    {
        Task<ParticipationResponseDto> CreateParticipationAsync(CreateParticipationRequestDto command);
        Task<ParticipationResponseDto?> GetParticipationByIdAsync(int id);
        Task<IEnumerable<ParticipationResponseDto>> GetAllParticipationsAsync();
        Task<IEnumerable<ParticipationResponseDto>> GetParticipationsByIdsAsync(int activityId, int userId);
        Task<IEnumerable<ParticipationResponseDto>> GetParticipationsByUserIdAsync(int userId);
        Task<IEnumerable<ParticipationResponseDto>> GetParticipationsByActivityIdAsync(int activityId);
        Task DeleteParticipationAsync(int id);
    }
}
