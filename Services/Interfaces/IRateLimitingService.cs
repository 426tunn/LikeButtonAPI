using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeButtonAPI.Services.Interfaces
{
    public interface IRateLimitingService
    {
        Task<bool> CheckRateLimitAsync(Guid userId, string action);

    }
}