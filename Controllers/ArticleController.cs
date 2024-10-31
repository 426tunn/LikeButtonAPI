using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LikeButtonAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILikeService _likeService;

        public ArticleController(IArticleService articleService, ILikeService likeservice)
        {
            _articleService = articleService;
            _likeService = likeservice;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Article>> CreateArticle([FromBody]Article articleData)
        {
            try
            {    
                var article = await _articleService.CreateArticleAsync(articleData);
                return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
            } catch (Exception ex)
            {
                throw new Exception($"Failed to create article: {ex.Message}");
            }
            }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return Ok(articles);
        }
    
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(Guid id)
        {
            var article = await _articleService.GetArticleAsync(id);
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpPost("{id}/toggle-like")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(Guid id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString))
                    return Unauthorized();

                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Invalid user ID.");
                }

                var isLiked = await _articleService.ToggleLikeAsync(id, userId, null);
                return Ok(new { isLiked });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpGet("{articleId}/likes")]
        [ResponseCache(Duration = 60)] // Cache the response for 60 seconds
        public async Task<IActionResult> GetLikeCount(Guid articleId)
        {
            try
                {
                    var likeCount = await _likeService.GetLikeCountAsync(articleId);
                    return Ok(new { likeCount });
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error getting like count: {ex.Message}");
                }
        }
    }
}