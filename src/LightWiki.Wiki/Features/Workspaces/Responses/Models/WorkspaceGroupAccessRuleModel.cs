using LightWiki.Domain.Enums;

namespace LightWiki.Features.Workspaces.Responses.Models;

public class WorkspaceGroupAccessRuleModel
{
    public int GroupId { get; set; }

    public int WorkspaceId { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}