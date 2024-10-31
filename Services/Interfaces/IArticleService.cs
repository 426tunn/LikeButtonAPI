using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;

namespace LikeButtonAPI.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleDto?> GetArticleAsync(Guid id);
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<Article> CreateArticleAsync(Article article);
        Task<bool> ToggleLikeAsync(Guid articleId, Guid userId, string? userIpAddress);
    }
}