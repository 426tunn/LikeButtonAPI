using System;
using System.Threading.Tasks;

namespace LikeButtonAPI.Services.Interfaces
{
    public interface ILikeService
    {
        Task<bool> ToggleLikeAsync(Guid articleId, Guid userId, string? userIpAddress);
        Task<int> GetLikeCountAsync(Guid articleId);
        Task<bool> HasUserLikedAsync(Guid articleId, Guid userId);
    }
}
