using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LikeButtonAPI.Data;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Repositories.Interfaces;
using LikeButtonAPI.Services.Implementation;
using LikeButtonAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace LikeButtonAPI.Repositories.Implementations
{
    public class LikeRepository : ILikeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LikeRepository> _logger;
        private readonly IDistributedCache _cache;
        private readonly IRateLimitingService _rateLimitingService;

        public LikeRepository(AppDbContext context, IDistributedCache cache, ILogger<LikeRepository> logger, IRateLimitingService rateLimitingService)
        {
            _context = context;
            _cache = cache;
            _rateLimitingService = rateLimitingService;
            _logger = logger;

        }

        public async Task<IResult> AddLikeAsync(Guid articleId, Guid userId, string? userIpAddress)
        {
            // Concurrency solution: First, use cache to handle like count quickly
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
                {
                    if (!await _rateLimitingService.CheckRateLimitAsync(userId, "like"))
                    {
                        return Results.BadRequest("Rate limit exceeded. Please try again later.");
                    }

                    var like = new Like
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ArticleId = articleId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Likes.AddAsync(like);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to add like: {ex.Message}");
                }
        }

           public async Task<IResult> RemoveLikeAsync(Guid articleId, Guid userId)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var like = await _context.Likes
                        .FirstOrDefaultAsync(l => l.ArticleId == articleId && l.UserId == userId);

                    if (like == null)
                    {
                        return Results.BadRequest("Like not found.");
                    }

                    _context.Likes.Remove(like);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to remove like: {ex.Message}");
                }
            }

        public async Task<int> GetLikeCountAsync(Guid articleId)
        {
            var cacheKey = $"article_likes_{articleId}";
            var cachedCountBytes = await _cache.GetAsync(cacheKey, new CancellationTokenSource(2000).Token);

            if (cachedCountBytes != null)
            {
                var cachedCountString = System.Text.Encoding.UTF8.GetString(cachedCountBytes);
                var cachedCount = JsonSerializer.Deserialize<int>(cachedCountString);
                return cachedCount;
            }

            // If not in cache, get from the database
            var count = await _context.Articles
                .Where(a => a.Id == articleId)
                .Select(a => a.LikeCount)
                .FirstOrDefaultAsync();

            await UpdateCacheAsync(articleId, count);

            return count;
        }

        public async Task<bool> HasUserLikedAsync(Guid articleId, Guid userId)
        {
            return await _context.Likes
                .AnyAsync(l => l.ArticleId == articleId && l.UserId == userId);
        }

        public async Task UpdateCacheAsync(Guid articleId, int likeCount)
        {
            try{
            var cacheKey = $"article_likes_{articleId}";
            var likeCountBytes = JsonSerializer.SerializeToUtf8Bytes(likeCount);
            
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // 5 seconds timeout
            
            try
            {
                await _cache.SetAsync(cacheKey, likeCountBytes, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                }, cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Setting cache for article {ArticleId} timed out.", articleId);
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the cache for article {ArticleId}.", articleId);
            }

        }

    }
}