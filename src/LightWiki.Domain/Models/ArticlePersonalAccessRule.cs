using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class ArticlePersonalAccessRule
    {
        public int Id { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public Article Article { get; set; }

        public int ArticleId { get; set; }

        public ArticleAccessRule ArticleAccessRule { get; set; }
    }
}