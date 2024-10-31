namespace LikeButtonAPI.Entities
{
    public class Like
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ArticleId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User? User { get; set; }
    public Article? Article { get; set; }
}
}
