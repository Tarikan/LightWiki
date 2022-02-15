using System;
using System.Collections.Generic;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class Workspace : ITrackable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public List<Article> Articles { get; set; }

        public List<WorkspaceAccess> WorkspaceAccesses { get; set; }

        public Article RootArticle { get; set; }

        public int? RootArticleId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}