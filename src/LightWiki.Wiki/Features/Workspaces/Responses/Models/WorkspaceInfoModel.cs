using System.Collections.Generic;

namespace LightWiki.Features.Workspaces.Responses.Models;

public class WorkspaceInfoModel
{
    public int Id { get; set; }

    public List<WorkspacePersonalAccessRuleModel> PersonalRules { get; set; }

    public List<WorkspaceGroupAccessRuleModel> GroupRules { get; set; }
}