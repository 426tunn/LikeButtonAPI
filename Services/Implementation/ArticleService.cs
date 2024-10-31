using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Repositories.Interfaces;
using LikeButtonAPI.Services.Interfaces;

namespace LikeButtonAPI.Services.Implementation
{
    public class ArticleService : IArticleService
    {
            private readonly IArticleRepository _articleRepository;
            private readonly ILikeService _likeService;
            private readonly ILogger<ArticleService> _logger;

        public ArticleService(IArticleRepository articleRepository, ILikeService likeService, ILogger<ArticleService> logger)
        {
            _articleRepository = articleRepository;
            _likeService = likeService;
            _logger = logger;
        }

        public async Task<Article> CreateArticleAsync(Article article)
        {
            article.Id = Guid.NewGuid();
            return await _articleRepository.CreateAsync(article);
        }

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllAsync();
        }


        public async Task<ArticleDto?> GetArticleAsync(Guid id)
        {
            return await _articleRepository.GetByIdAsync(id);
        }

        public async Task<bool> ToggleLikeAsync(Guid articleId, Guid userId, string? userIpAddress)
            {
                try
                {
                    if (!await _articleRepository.ExistsAsync(articleId))
                    {
                        throw new KeyNotFoundException($"Article with ID {articleId} not found.");
                    }

                    return await _likeService.ToggleLikeAsync(articleId, userId, null);
                }
                catch (Exception ex)
                {
                _logger.LogError(ex, "An error occurred while toggling article like.");
                throw;
                }
            }

    }
}