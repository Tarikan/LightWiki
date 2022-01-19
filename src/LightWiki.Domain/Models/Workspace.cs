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

        public List<Article> Articles { get; set; }

        public WorkspaceAccessRule WorkspaceAccessRule { get; set; }

        public List<WorkspacePersonalAccessRule> PersonalAccessRules { get; set; }

        public List<WorkspaceGroupAccessRule> GroupAccessRules { get; set; }

        public string ArticleTreeId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}