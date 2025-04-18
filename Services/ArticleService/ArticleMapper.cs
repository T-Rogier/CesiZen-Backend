using CesiZen_Backend.Dtos.ArticleDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ArticleService
{
    public class ArticleMapper
    {
        public static ArticleDto ToDto(Article article)
        {
            return new ArticleDto(
                article.Id,
                article.Title,
                article.Content,
                article.MenuId
            );
        }
    }
}
