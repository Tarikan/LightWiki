using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class RemoveWorkspaceAccessHandler : IRequestHandler<RemoveWorkspaceAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveWorkspaceAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveWorkspaceAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.WorkspaceAccesses
            .SingleAsync(
                a => a.WorkspaceId == request.WorkspaceId &&
                     a.PartyId == request.PartyId,
                cancellationToken);

        _wikiContext.WorkspaceAccesses.Remove(access);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}