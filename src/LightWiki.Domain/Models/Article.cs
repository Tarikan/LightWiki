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

        public string Slug { get; set; }

        public List<ArticleVersion> Versions { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public int? ParentArticleId { get; set; }

        public Article ParentArticle { get; set; }

        public List<Article> ChildArticles { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ArticleAccess> ArticleAccesses { get; set; }

        public Workspace RootedWorkspace { get; set; }
    }
}