using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeButtonAPI.Repositories.Interfaces
{
    public interface ILikeRepository
    {
        Task<IResult> AddLikeAsync(Guid articleId, Guid userId, string? userIpAddress);
        Task<IResult> RemoveLikeAsync(Guid articleId, Guid userId);
        Task<int> GetLikeCountAsync(Guid articleId);
        Task<bool> HasUserLikedAsync(Guid articleId, Guid userId);
        Task UpdateCacheAsync(Guid articleId, int likeCount);       
    }
}