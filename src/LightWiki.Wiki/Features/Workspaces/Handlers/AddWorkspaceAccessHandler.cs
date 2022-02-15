using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class AddWorkspaceAccessHandler : IRequestHandler<AddWorkspaceAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddWorkspaceAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddWorkspaceAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.WorkspaceAccesses
            .SingleOrDefaultAsync(
                a => a.WorkspaceId == request.WorkspaceId && a.PartyId == request.PartyId,
                cancellationToken);

        access ??= new WorkspaceAccess
        {
            WorkspaceId = request.WorkspaceId,
            PartyId = request.PartyId,
        };

        access.WorkspaceAccessRule = request.WorkspaceAccessRule;

        _wikiContext.WorkspaceAccesses.Update(access);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}