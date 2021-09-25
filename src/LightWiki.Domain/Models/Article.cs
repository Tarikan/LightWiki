using System;
using System.Collections.Generic;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class Article : ITrackable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ArticleVersion> Versions { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public ArticleVisibility ArticleVisibility { get; set; }

        public ArticleModificationAccess ArticleModificationAccess { get; set; }
    }
}