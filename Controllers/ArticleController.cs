using CesiZen_Backend.Dtos.ArticleDtos;
using CesiZen_Backend.Services.ArticleService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/articles")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _ArticleService;

        public ArticleController(IArticleService articleService)
        {
            _ArticleService = articleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto command)
        {
            ArticleDto article = await _ArticleService.CreateArticleAsync(command);
            return CreatedAtAction(nameof(GetArticleById), new { id = article.Id }, article);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            IEnumerable<ArticleDto> articles = await _ArticleService.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            ArticleDto? article = await _ArticleService.GetArticleByIdAsync(id);
            return article is null ? NotFound(new { Message = $"Article with ID {id} not found." }) : Ok(article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleDto command)
        {
            await _ArticleService.UpdateArticleAsync(id, command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _ArticleService.DeleteArticleAsync(id);
            return NoContent();
        }
    }
}
