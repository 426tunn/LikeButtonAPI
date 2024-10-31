namespace LikeButtonAPI.Entities {
    public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; } = 0;  // Denormalized count for quick access
    public ICollection<Like>? Likes { get; set; }
}
}