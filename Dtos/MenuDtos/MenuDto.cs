namespace CesiZen_Backend.Dtos.MenuDtos
{
    public record MenuDto(int Id, string Title, int HierarchyLevel, int? ParentId);
}
