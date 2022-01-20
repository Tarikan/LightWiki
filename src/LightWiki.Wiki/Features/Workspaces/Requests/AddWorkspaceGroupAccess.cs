using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class AddWorkspaceGroupAccess : IRequest<OneOf<Success, Fail>>
{
    public int WorkspaceId { get; set; }

    public int GroupId { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}