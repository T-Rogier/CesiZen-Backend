using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Dtos.ArticleDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.ArticleService
{
    public class ArticleMapper
    {
        public static FullArticleResponseDto ToFullDto(Article article)
        {
            return new FullArticleResponseDto(
                article.Id,
                article.Title,
                article.Content,
                article.MenuId
            );
        }

        public static SimpleArticleResponseDto ToSimpleDto(Article article)
        {
            return new SimpleArticleResponseDto(
                article.Id,
                article.Title
            );
        }

        public static ArticleListResponseDto ToListDto(List<Article> articles, int totalCount, int pageNumber = 1, int pageSize = 10)
        {
            return new ArticleListResponseDto(
                articles.Select(ToSimpleDto),
                pageNumber,
                pageSize,
                totalCount,
                totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0)
            );
        }
    }
}
