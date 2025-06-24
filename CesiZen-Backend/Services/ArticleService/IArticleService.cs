using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ArticleDtos;

namespace CesiZen_Backend.Services.ArticleService
{
    public interface IArticleService
    {
        Task<FullArticleResponseDto> CreateArticleAsync(CreateArticleRequestDto command);
        Task<FullArticleResponseDto?> GetArticleByIdAsync(int id);
        Task<ArticleListResponseDto> GetAllArticlesAsync();
        Task<ArticleListResponseDto> GetArticlesByMenuAsync(int menuId, PagingRequestDto paging);
        Task<ArticleListResponseDto> FindInArticleAsync(FindInArticleRequestDto filter);
        Task UpdateArticleAsync(int id, UpdateArticleRequestDto command);
        Task DeleteArticleAsync(int id);
    }
}
