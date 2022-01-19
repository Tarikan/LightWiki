using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class RemoveWorkspace : IRequest<OneOf<Success, Fail>>
{
    public int WorkspaceId { get; set; }
}