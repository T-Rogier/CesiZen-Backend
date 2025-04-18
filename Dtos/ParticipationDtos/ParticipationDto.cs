namespace CesiZen_Backend.Dtos.ParticipationDtos
{
    public record ParticipationDto(
        int Id, 
        int UserId, 
        int ActivityId, 
        DateTime ParticipationDate, 
        TimeSpan Duration
    );
}
