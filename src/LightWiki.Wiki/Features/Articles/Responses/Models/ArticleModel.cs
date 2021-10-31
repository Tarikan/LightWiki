using System;
using System.Collections.Generic;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;

namespace LightWiki.Features.Articles.Responses.Models
{
    public class ArticleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public ArticleAccessRule GlobalAccessRule { get; set; }

        public List<ArticleGroupAccessRule> GroupAccessRules { get; set; }

        public List<ArticlePersonalAccessRule> PersonalAccessRules { get; set; }
    }
}