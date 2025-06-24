namespace CesiZen_Backend.Dtos.MenuDtos
{
    public record CreateMenuRequestDto(
        string Title,
        int? ParentId
    );

    public record UpdateMenuRequestDto(
        string Title,
        int? ParentId
    );
}
