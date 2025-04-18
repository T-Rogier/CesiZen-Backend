namespace CesiZen_Backend.Dtos.MenuDtos
{
    public record CreateMenuDto(
        string Title,
        int? ParentId
    );
}
