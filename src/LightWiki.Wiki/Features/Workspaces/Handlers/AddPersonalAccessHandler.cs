using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class AddPersonalAccessHandler : IRequestHandler<AddPersonalAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddPersonalAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddPersonalAccess request, CancellationToken cancellationToken)
    {
        var workspacePersonalAccessRule = await _wikiContext.WorkspacePersonalAccessRules
            .SingleOrDefaultAsync(
                gar => gar.UserId == request.UserId &&
                       gar.WorkspaceId == request.WorkspaceId,
                cancellationToken);

        workspacePersonalAccessRule ??= new WorkspacePersonalAccessRule
        {
            UserId = request.UserId,
            WorkspaceId = request.WorkspaceId,
        };

        workspacePersonalAccessRule.WorkspaceAccessRule = request.WorkspaceAccessRule;

        _wikiContext.WorkspacePersonalAccessRules.Update(workspacePersonalAccessRule);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}