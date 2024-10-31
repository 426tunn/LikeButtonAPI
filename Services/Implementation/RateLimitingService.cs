using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LikeButtonAPI.Services.Implementation
{
    public class RateLimitingService : IRateLimitingService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RateLimitingService> _logger;
        private readonly RateLimitingConfiguration _config;

        public RateLimitingService(IConnectionMultiplexer redis, ILogger<RateLimitingService> logger, IOptions<RateLimitingConfiguration> config)
        {
            _redis = redis;
            _logger = logger;
            _config = config.Value;
        }

        public async Task<bool> CheckRateLimitAsync(Guid userId, string action)
        {
            var db = _redis.GetDatabase();
            var now = DateTime.UtcNow;

            // Keys for different time windows
            var minuteKey = $"ratelimit:{action}:minute:{userId}:{now:yyyy-MM-dd-HH-mm}";
            var hourKey = $"ratelimit:{action}:hour:{userId}:{now:yyyy-MM-dd-HH}";
            var dayKey = $"ratelimit:{action}:day:{userId}:{now:yyyy-MM-dd}";

            try
            {
                // Increment counters without transactions
                var minuteCount = await db.StringIncrementAsync(minuteKey);
                var hourCount = await db.StringIncrementAsync(hourKey);
                var dayCount = await db.StringIncrementAsync(dayKey);

                // Set expiration for keys if they don't exist
                await db.KeyExpireAsync(minuteKey, TimeSpan.FromMinutes(1));
                await db.KeyExpireAsync(hourKey, TimeSpan.FromHours(1));
                await db.KeyExpireAsync(dayKey, TimeSpan.FromDays(1));

                // Check if user has exceeded any limits
                if (minuteCount > _config.PerMinuteLimit || 
                    hourCount > _config.PerHourLimit || 
                    dayCount > _config.PerDayLimit)
                {
                    _logger.LogWarning("Rate limit exceeded for user {UserId} on action {Action}", userId, action);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking rate limit for user {UserId}", userId);
                return false; // Fail closed - if we can't check rate limit, don't allow the action
            }
        }

    }
}