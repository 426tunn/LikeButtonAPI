using System;
using System.Threading.Tasks;
using LikeButtonAPI.Repositories.Implementations;
using LikeButtonAPI.Repositories.Interfaces;
using LikeButtonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LikeButtonAPI.Services.Implementation
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<LikeService> _logger;

            

        public LikeService(ILikeRepository likeRepository, ILogger<LikeService> logger, IArticleRepository articleRepository)
        {
            _likeRepository = likeRepository;
            _articleRepository = articleRepository;
            _logger = logger;
        }


        public async Task<bool> ToggleLikeAsync(Guid articleId, Guid userId, string? userIpAddress)
        {
            try
            {
                var hasLiked = await _likeRepository.HasUserLikedAsync(articleId, userId);

                if (!hasLiked)
                {
                    await _likeRepository.AddLikeAsync(articleId, userId, userIpAddress);
                    await _articleRepository.IncrementLikeCountAsync(articleId);
                }
                else
                {
                    await _likeRepository.RemoveLikeAsync(articleId, userId);
                    await _articleRepository.DecrementLikeCountAsync(articleId);
                }

                var newCount = await _likeRepository.GetLikeCountAsync(articleId);
                await _likeRepository.UpdateCacheAsync(articleId, newCount);
                return !hasLiked;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while toggling like status.");
                throw;
            }
        }

        public async Task<int> GetLikeCountAsync(Guid articleId)
        {
            try
            {
                return await _likeRepository.GetLikeCountAsync(articleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving like count for article {ArticleId}", articleId);
                throw new Exception("Failed to retrieve like count", ex);
            }
        }

        public async Task<bool> HasUserLikedAsync(Guid articleId, Guid userId)
        {
            try
            {
                return await _likeRepository.HasUserLikedAsync(articleId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if user {UserId} liked article {ArticleId}", userId, articleId);
                throw new Exception("Failed to check user like status", ex);
            }
        }



    }
}
