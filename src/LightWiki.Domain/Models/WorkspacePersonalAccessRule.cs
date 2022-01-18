using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class WorkspacePersonalAccessRule
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
    }
}