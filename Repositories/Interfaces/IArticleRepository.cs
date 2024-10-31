using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;

namespace LikeButtonAPI.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<ArticleDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<Article>> GetAllAsync();
        Task<Article> CreateAsync(Article article);
        Task<bool> ExistsAsync(Guid id);
        Task IncrementLikeCountAsync(Guid id);
        Task DecrementLikeCountAsync(Guid id);
    }
}