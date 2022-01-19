using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class UpdateWorkspaceHandler : IRequestHandler<UpdateWorkspace, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public UpdateWorkspaceHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateWorkspace request, CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces.FindAsync(request.Id);

        workspace.Name = request.Name;
        _wikiContext.Workspaces.Update(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}