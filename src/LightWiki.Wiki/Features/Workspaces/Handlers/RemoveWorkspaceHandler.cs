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
    private readonly IWorkspaceTreeRepository _workspaceTreeRepository;

    public RemoveWorkspaceHandler(WikiContext wikiContext, IWorkspaceTreeRepository workspaceTreeRepository)
    {
        _wikiContext = wikiContext;
        _workspaceTreeRepository = workspaceTreeRepository;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveWorkspace request, CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces.FindAsync(request.WorkspaceId);

        await _workspaceTreeRepository.Remove(workspace.ArticleTreeId);

        _wikiContext.Workspaces.Remove(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}