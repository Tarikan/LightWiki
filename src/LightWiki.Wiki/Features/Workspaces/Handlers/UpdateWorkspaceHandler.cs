using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;
using Slugify;

namespace LightWiki.Features.Workspaces.Handlers;

public class UpdateWorkspaceHandler : IRequestHandler<UpdateWorkspace, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly ISlugHelper _slugHelper;

    public UpdateWorkspaceHandler(WikiContext wikiContext, ISlugHelper slugHelper)
    {
        _wikiContext = wikiContext;
        _slugHelper = slugHelper;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateWorkspace request, CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces.FindAsync(request.Id);

        workspace.Name = request.Name;
        workspace.Slug = _slugHelper.GenerateSlug(request.Name);
        _wikiContext.Workspaces.Update(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}