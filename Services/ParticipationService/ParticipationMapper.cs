using CesiZen_Backend.Dtos.ParticipationDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ParticipationService
{
    public class ParticipationMapper
    {
        public static ParticipationDto ToDto(Participation participation)
        {
            return new ParticipationDto(
                participation.Id,
                participation.UserId,
                participation.ActivityId,
                participation.ParticipationDate,
                participation.Duration
            );
        }
    }
}
