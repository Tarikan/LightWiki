using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class GetWorkspaceBySlug : IRequest<OneOf<WorkspaceModel, Fail>>
{
    public string Slug { get; set; }
}