using System;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class ArticleAccess : ITrackable
    {
        public int Id { get; set; }

        public int PartyId { get; set; }

        public Party Party { get; set; }

        public ArticleAccessRule ArticleAccessRule { get; set; }

        public Article Article { get; set; }

        public int ArticleId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}