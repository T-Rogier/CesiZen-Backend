namespace CesiZen_Backend.Dtos.MenuDtos
{
    public record UpdateMenuDto(
        string Title,
        int? ParentId
    );
}
