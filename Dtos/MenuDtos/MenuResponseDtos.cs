namespace CesiZen_Backend.Dtos.MenuDtos
{
    public record MenuResponseDto(
        int Id,
        string Title,
        int HierarchyLevel,
        int? ParentId
    );
}
