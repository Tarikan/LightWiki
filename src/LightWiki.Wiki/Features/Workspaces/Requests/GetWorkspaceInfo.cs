using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class GetWorkspaceInfo : IRequest<OneOf<WorkspaceInfoModel, Fail>>
{
    public int WorkspaceId { get; set; }
}