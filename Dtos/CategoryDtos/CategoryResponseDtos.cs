namespace CesiZen_Backend.Dtos.CategoryDtos
{
    public record CategoryResponseDto(
        int Id,
        string Name,
        string IconLink,
        bool Deleted
    );

    public record CategoryListResponseDto(
        IEnumerable<CategoryResponseDto> Categories,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
