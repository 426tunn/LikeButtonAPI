using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.Entities;
using Microsoft.EntityFrameworkCore;


namespace LikeButtonAPI.Data
{
    public class AppDbContext : DbContext
    {
    public AppDbContext(){}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<Like> Likes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensures that one user can only like an article once
        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.ArticleId })
            .IsUnique();

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Article)
            .WithMany(a => a.Likes)
            .HasForeignKey(l => l.ArticleId);
    }


    }
}