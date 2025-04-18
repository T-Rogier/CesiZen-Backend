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

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto command)
        {           
            var menu = await _dbContext.Menus.FindAsync(command.MenuId);

            if (menu == null)
                throw new ArgumentNullException($"Invalid ParentMenu Id.");

            var article = Article.Create(command.Title, command.Content, menu);

            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();

            return ArticleMapper.ToDto(article);
        }

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        {
            return await _dbContext.Articles
                .AsNoTracking()
                .Select(m => ArticleMapper.ToDto(m))
                .ToListAsync();
        }

        public async Task<ArticleDto?> GetArticleByIdAsync(int id)
        {
            var article = await _dbContext.Articles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
                return null;

            return ArticleMapper.ToDto(article);
        }

        public async Task UpdateArticleAsync(int id, UpdateArticleDto command)
        {
            var articleToUpdate = await _dbContext.Articles.FindAsync(id);
            if (articleToUpdate is null)
                throw new ArgumentNullException($"Invalid Article Id.");

            var menu = await _dbContext.Menus.FindAsync(command.MenuId);

            if (menu == null)
                throw new ArgumentNullException($"Invalid ParentMenu Id.");

            articleToUpdate.Update(command.Title, command.Content, menu);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteArticleAsync(int id)
        {
            var articleToDelete = await _dbContext.Articles.FindAsync(id);
            if (articleToDelete != null)
            {
                _dbContext.Articles.Remove(articleToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
