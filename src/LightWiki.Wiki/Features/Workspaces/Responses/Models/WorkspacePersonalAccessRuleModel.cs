using LightWiki.Domain.Enums;

namespace LightWiki.Features.Workspaces.Responses.Models;

public class WorkspacePersonalAccessRuleModel
{
    public int UserId { get; set; }

    public int WorkspaceId { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}