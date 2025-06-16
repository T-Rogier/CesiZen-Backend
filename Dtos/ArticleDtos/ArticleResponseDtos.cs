namespace CesiZen_Backend.Dtos.ArticleDtos
{
    public record FullArticleResponseDto(
        int Id,
        string Title,
        string Content,
        int MenuId
    );

    public record SimpleArticleResponseDto(
        int Id,
        string Title
    );

    public record ArticleListResponseDto(
        IEnumerable<SimpleArticleResponseDto> Articles,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
