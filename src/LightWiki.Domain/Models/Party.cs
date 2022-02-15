using System.Collections.Generic;
using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class Party
    {
        public int Id { get; set; }

        public PartyType PartyType { get; set; }

        public List<ArticleAccess> ArticleAccesses { get; set; }

        public List<WorkspaceAccess> WorkspaceAccesses { get; set; }

        public List<User> Users { get; set; }

        public List<Group> Groups { get; set; }
    }
}