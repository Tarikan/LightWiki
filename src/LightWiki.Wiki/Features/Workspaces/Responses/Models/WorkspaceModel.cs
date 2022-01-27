using LightWiki.Domain.Enums;

namespace LightWiki.Features.Workspaces.Responses.Models;

public class WorkspaceModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string WorkspaceRootArticleSlug { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}