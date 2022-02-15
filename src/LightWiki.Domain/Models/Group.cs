using System.Collections.Generic;
using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public GroupAccessRule GroupAccessRule { get; set; }

        public List<User> Users { get; set; }

        public GroupType GroupType { get; set; }

        public Party Party { get; set; }

        public int PartyId { get; set; }

        public List<ArticleGroupAccessRule> ArticleGroupAccessRules { get; set; }

        public List<GroupPersonalAccessRule> GroupPersonalAccessRules { get; set; }

        public List<WorkspaceGroupAccessRule> WorkspaceGroupAccessRules { get; set; }
    }
}