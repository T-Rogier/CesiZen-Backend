using CesiZen_Backend.Dtos.ArticleDtos;

namespace CesiZen_Backend.Dtos.MenuDtos
{
    public record SimpleMenuResponseDto(
        int Id,
        string Title,
        int HierarchyLevel,
        int? ParentId
    );

    public record FullMenuResponseDto(
        int Id,
        string Title,
        int HierarchyLevel,
        ICollection<FullMenuResponseDto>? ChildMenus,
        ICollection<SimpleArticleResponseDto>? ChildArticles
    );
}
