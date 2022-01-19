using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class RemovePersonalAccessHandler : IRequestHandler<RemovePersonalAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemovePersonalAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemovePersonalAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.WorkspacePersonalAccessRules
            .SingleAsync(
                a => a.UserId == request.UserId &&
                     a.WorkspaceId == request.WorkspaceId,
                cancellationToken);

        _wikiContext.WorkspacePersonalAccessRules.Remove(access);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}