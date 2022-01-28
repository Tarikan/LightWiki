using System;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class ArticleVersion : ITrackable
    {
        public int Id { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}