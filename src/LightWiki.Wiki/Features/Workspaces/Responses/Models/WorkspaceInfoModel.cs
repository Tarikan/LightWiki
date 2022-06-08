using System.Collections.Generic;

namespace LightWiki.Features.Workspaces.Responses.Models;

public class WorkspaceInfoModel
{
    public int Id { get; set; }

    public List<WorkspaceAccessModel> WorkspaceAccess { get; set; }

    public string Slug { get; set; }
}