using System;
using LightWiki.Domain.Enums;

namespace LightWiki.Features.Articles.Responses.Models
{
    public class ArticleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public ArticleVisibility ArticleVisibility { get; set; }

        public ArticleModificationAccess ArticleModificationAccess { get; set; }
    }
}