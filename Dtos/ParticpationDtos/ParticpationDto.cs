namespace CesiZen_Backend.Dtos.ParticpationDtos
{
    public record ParticpationDto(int Id, int UserId, int ActivityId, DateTime ParticipationDate, TimeSpan Duration);
}
