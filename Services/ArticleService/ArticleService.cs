using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ArticleDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.ArticleService;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.Articleservice
{
    public class ArticleService : IArticleService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<ArticleService> _logger;
        public ArticleService(CesiZenDbContext dbContext, ILogger<ArticleService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<FullArticleResponseDto> CreateArticleAsync(CreateArticleRequestDto command)
        {           
            Menu? menu = await _dbContext.Menus.FindAsync(command.MenuId);

            if (menu is null)
                throw new ArgumentNullException($"Invalid ParentMenu Id.");

            Article article = Article.Create(command.Title, command.Content, menu);

            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();

            return ArticleMapper.ToFullDto(article);
        }

        public async Task<ArticleListResponseDto> GetAllArticlesAsync()
        {
            List<Article> articles = await _dbContext.Articles
                .AsNoTracking()
                .ToListAsync();
            return ArticleMapper.ToListDto(articles, articles.Count);
        }

        public async Task<ArticleListResponseDto> GetArticlesByMenuAsync(int menuId, PagingRequestDto paging)
        {
            int pageNumber = Math.Max(1, paging.PageNumber);
            int pageSize = Math.Max(1, paging.PageSize);

            int totalCount = await _dbContext.Articles.CountAsync();

            List<Article> articles = await _dbContext.Articles
                .AsNoTracking()
                .Where(a => a.MenuId == menuId)
                .ToListAsync();
            return ArticleMapper.ToListDto(articles, totalCount, pageNumber, pageSize);
        }

        public async Task<FullArticleResponseDto?> GetArticleByIdAsync(int id)
        {
            Article? article = await _dbContext.Articles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
                return null;

            return ArticleMapper.ToFullDto(article);
        }

        public async Task UpdateArticleAsync(int id, UpdateArticleRequestDto command)
        {
            Article? articleToUpdate = await _dbContext.Articles.FindAsync(id);
            if (articleToUpdate is null)
                throw new ArgumentNullException($"Invalid Article Id.");

            Menu? menu = await _dbContext.Menus.FindAsync(command.MenuId);

            if (menu == null)
                throw new ArgumentNullException($"Invalid ParentMenu Id.");

            articleToUpdate.Update(command.Title, command.Content, menu);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteArticleAsync(int id)
        {
            Article? articleToDelete = await _dbContext.Articles.FindAsync(id);
            if (articleToDelete != null)
            {
                _dbContext.Articles.Remove(articleToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
