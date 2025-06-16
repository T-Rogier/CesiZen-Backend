namespace CesiZen_Backend.Dtos
{
    public record PagingRequestDto(
        int PageNumber = 1,
        int PageSize = 10
    );
}
