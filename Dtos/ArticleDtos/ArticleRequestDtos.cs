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
}
