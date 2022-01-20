using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class RemoveGroupAccessHandler : IRequestHandler<RemoveWorkspaceGroupAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveGroupAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveWorkspaceGroupAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.WorkspaceGroupAccessRules
            .SingleAsync(
                a => a.GroupId == request.GroupId &&
                     a.WorkspaceId == request.WorkspaceId,
                cancellationToken);

        _wikiContext.WorkspaceGroupAccessRules.Remove(access);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}