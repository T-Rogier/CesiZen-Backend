namespace CesiZen_Backend.Dtos.ArticleDtos
{
    public record UpdateArticleDto(
        string Title,
        string Content,
        int MenuId
    );
}
