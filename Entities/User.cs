using System;
namespace LikeButtonAPI.Entities
{
	public class User
{
    public Guid Id { get; set; }
    public  string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public ICollection<Like>? Likes { get; set; }
}
}

