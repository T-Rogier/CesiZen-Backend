namespace CesiZen_Backend.Dtos.ParticipationDtos
{
    public record CreateParticipationDto(
        int UserId,
        int ActivityId,
        DateTime ParticipationDate,
        TimeSpan Duration
    );
}
