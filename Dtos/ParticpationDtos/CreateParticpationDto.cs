namespace CesiZen_Backend.Dtos.ParticpationDtos
{
    public record CreateParticpationDto(int UserId, int ActivityId, DateTime ParticipationDate, TimeSpan Duration);
}
