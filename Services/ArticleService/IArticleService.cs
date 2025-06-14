using CesiZen_Backend.Dtos.ArticleDtos;

namespace CesiZen_Backend.Services.ArticleService
{
    public interface IArticleService
    {
        Task<ArticleResponseDto> CreateArticleAsync(CreateArticleRequestDto command);
        Task<ArticleResponseDto?> GetArticleByIdAsync(int id);
        Task<IEnumerable<ArticleResponseDto>> GetAllArticlesAsync();
        Task UpdateArticleAsync(int id, UpdateArticleRequestDto command);
        Task DeleteArticleAsync(int id);
    }
}
