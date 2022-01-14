using System.Collections.Generic;
using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public GroupAccessRule GroupAccessRule { get; set; }

        public List<User> Users { get; set; }

        public List<ArticleGroupAccessRule> ArticleGroupAccessRules { get; set; }

        public List<GroupPersonalAccessRule> GroupPersonalAccessRules { get; set; }
    }
}