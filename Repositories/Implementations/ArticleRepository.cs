using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.Data;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LikeButtonAPI.Repositories.Implementations
{
    public class ArticleRepository : IArticleRepository
    {

        private readonly AppDbContext _context;
        private readonly ILogger<ArticleRepository> _logger;

        public ArticleRepository(AppDbContext context, ILogger<ArticleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Article> CreateAsync(Article article)
        {
            article.CreatedAt = DateTime.UtcNow;
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }


        public async Task<ArticleDto?> GetByIdAsync(Guid id)
        {
                var article = await _context.Articles
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    LikeCount = a.LikeCount
                })
                .FirstOrDefaultAsync(a => a.Id == id);

            return article;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Articles.AnyAsync(a => a.Id == id);
        }

        public async Task IncrementLikeCountAsync(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                article.LikeCount++;
                _context.Entry(article).Property(x => x.LikeCount).IsModified = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"LikeCount after increment: {article.LikeCount}");
            }
        }

        public async Task DecrementLikeCountAsync(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                article.LikeCount = Math.Max(0, article.LikeCount - 1);
                _context.Entry(article).Property(x => x.LikeCount).IsModified = true;
                await _context.SaveChangesAsync();
            }
        }




        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Articles
                .Include(a => a.Likes)
                .ToListAsync();
        }
    }
}