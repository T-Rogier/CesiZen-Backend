namespace CesiZen_Backend.Dtos.ArticleDtos
{
    public record CreateArticleRequestDto(
        string Title,
        string Content,
        int MenuId
    );

    public record UpdateArticleRequestDto(
        string Title,
        string Content,
        int MenuId
    );

    public record FindInArticleRequestDto(
        string? Query,
        int PageNumber = 1,
        int PageSize = 10
    );
}
