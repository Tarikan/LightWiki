using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class CreateWorkspace : IRequest<OneOf<SuccessWithId<int>, Fail>>
{
    public string Name { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}