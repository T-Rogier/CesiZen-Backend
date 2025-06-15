using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ArticleDtos;
using CesiZen_Backend.Filters;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ArticleService;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequestDto command)
        {
            FullArticleResponseDto article = await _ArticleService.CreateArticleAsync(command);
            return CreatedAtAction(nameof(GetArticleById), new { id = article.Id }, article);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            ArticleListResponseDto articles = await _ArticleService.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("byMenu/{menuId}")]
        public async Task<IActionResult> GetArticlesByMenu(int menuId, [FromQuery] PagingRequestDto paging)
        {
            ArticleListResponseDto articles = await _ArticleService.GetArticlesByMenuAsync(menuId, paging);
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            FullArticleResponseDto? article = await _ArticleService.GetArticleByIdAsync(id);
            return article is null ? NotFound(new { Message = $"Article with ID {id} not found." }) : Ok(article);
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleRequestDto command)
        {
            await _ArticleService.UpdateArticleAsync(id, command);
            return NoContent();
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _ArticleService.DeleteArticleAsync(id);
            return NoContent();
        }
    }
}
