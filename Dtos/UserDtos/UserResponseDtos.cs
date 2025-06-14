namespace CesiZen_Backend.Dtos.UserDtos
{
    public record FullUserResponseDto(
        int Id,
        string Username,
        string Email,
        bool Disabled,
        string Role
    );

    public record SimpleUserResponseDto(
        int Id,
        string Username
    );

    public record UserListResponseDto(
        IEnumerable<SimpleUserResponseDto> Users,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
