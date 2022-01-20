using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class RemoveWorkspaceHandler : IRequestHandler<RemoveWorkspace, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveWorkspaceHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveWorkspace request, CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces.FindAsync(request.WorkspaceId);

        _wikiContext.Workspaces.Remove(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}