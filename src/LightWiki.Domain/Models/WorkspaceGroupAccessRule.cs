using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class WorkspaceGroupAccessRule
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public Group Group { get; set; }

        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
    }
}