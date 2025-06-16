using CesiZen_Backend.Dtos.ParticipationDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ParticipationService
{
    public class ParticipationMapper
    {
        public static ParticipationResponseDto ToDto(Participation participation)
        {
            return new ParticipationResponseDto(
                participation.Id,
                participation.UserId,
                participation.ActivityId,
                participation.ParticipationDate,
                participation.Duration
            );
        }
    }
}
