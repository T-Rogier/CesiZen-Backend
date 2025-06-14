namespace CesiZen_Backend.Dtos.ParticipationDtos
{
    public record CreateParticipationRequestDto(
        int UserId,
        int ActivityId,
        DateTime ParticipationDate,
        TimeSpan Duration
    );
}
