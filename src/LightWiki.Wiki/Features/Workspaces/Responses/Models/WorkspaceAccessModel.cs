using LightWiki.Domain.Enums;

namespace LightWiki.Features.Workspaces.Responses.Models;

public class WorkspaceAccessModel
{
    public int WorkspaceId { get; set; }

    public int PartyId { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}