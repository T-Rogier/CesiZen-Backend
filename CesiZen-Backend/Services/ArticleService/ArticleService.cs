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

            IQueryable<Article> query = _dbContext.Articles
                .AsNoTracking()
                .Where(a => a.MenuId == menuId);

            int totalCount = await query.CountAsync();

            List<Article> articles = await query
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ArticleMapper.ToListDto(articles, totalCount, pageNumber, pageSize);
        }

        public async Task<FullArticleListResponseDto> FindInArticleAsync(FindInArticleRequestDto filter)
        {
            int pageNumber = Math.Max(1, filter.PageNumber);
            int pageSize = Math.Max(1, filter.PageSize);

            IQueryable<Article> query = _dbContext.Articles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                query = query.Where(a =>
                  a.Title.ToLower().Contains(filter.Query.ToLower()) ||
                  a.Content.ToLower().Contains(filter.Query.ToLower()));
            }

            int totalCount = await query.CountAsync();

            List<Article> articles = await query
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ArticleMapper.ToListFullDto(articles, totalCount, pageNumber, pageSize);
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
            _dbContext.Entry(articleToUpdate).Property(c => c.Updated).IsModified = true;
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
