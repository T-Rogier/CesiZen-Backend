namespace CesiZen_Backend.Dtos.ArticleDtos
{
    public record ArticleDto(
        int Id,
        string Title,
        string Content,
        int MenuId
    );
}
