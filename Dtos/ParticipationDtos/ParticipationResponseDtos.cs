namespace CesiZen_Backend.Dtos.ParticipationDtos
{
    public record ParticipationResponseDto(
        int Id, 
        int UserId, 
        int ActivityId, 
        DateTime ParticipationDate, 
        TimeSpan Duration
    );
}
