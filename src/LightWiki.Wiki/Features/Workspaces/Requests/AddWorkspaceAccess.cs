using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class AddWorkspaceAccess : IRequest<OneOf<Success, Fail>>
{
    public int WorkspaceId { get; set; }

    public int PartyId { get; set; }

    public WorkspaceAccessRule WorkspaceAccessRule { get; set; }
}