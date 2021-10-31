using System.Collections.Generic;

namespace LightWiki.Domain.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }

        public List<ArticleGroupAccessRule> ArticleGroupAccessRules { get; set; }
    }
}