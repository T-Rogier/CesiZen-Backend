using CesiZen_Backend.Dtos.ArticleDtos;

namespace CesiZen_Backend.Services.ArticleService
{
    public interface IArticleService
    {
        Task<ArticleDto> CreateArticleAsync(CreateArticleDto command);
        Task<ArticleDto?> GetArticleByIdAsync(int id);
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
        Task UpdateArticleAsync(int id, UpdateArticleDto command);
        Task DeleteArticleAsync(int id);
    }
}
