using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class ArticleGroupAccessRule
    {
        public int Id { get; set; }

        public Group Group { get; set; }

        public int GroupId { get; set; }

        public Article Article { get; set; }

        public int ArticleId { get; set; }

        public ArticleAccessRule ArticleAccessRule { get; set; }
    }
}