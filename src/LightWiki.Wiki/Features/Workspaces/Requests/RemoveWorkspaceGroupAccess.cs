using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class RemoveWorkspaceGroupAccess : IRequest<OneOf<Success, Fail>>
{
    public int WorkspaceId { get; set; }

    public int GroupId { get; set; }
}