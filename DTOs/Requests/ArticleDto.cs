using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeButtonAPI.DTOs.Requests
{
    public class ArticleDto
    {
            public Guid Id {get; set; }
            public string Title { get; set; } = null!;
            public int LikeCount { get; set; }
    }
}