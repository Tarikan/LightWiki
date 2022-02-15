using System;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class WorkspaceAccess : ITrackable
    {
        public int Id { get; set; }

        public Workspace Workspace { get; set; }

        public int WorkspaceId { get; set; }

        public Party Party { get; set; }

        public int PartyId { get; set; }

        public WorkspaceAccessRule WorkspaceAccessRule { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}