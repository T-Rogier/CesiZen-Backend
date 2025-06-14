namespace CesiZen_Backend.Dtos.ArticleDtos
{
    public record ArticleResponseDto(
        int Id,
        string Title,
        string Content,
        int MenuId
    );
}
