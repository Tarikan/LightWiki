using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class AddGroupAccessHandler : IRequestHandler<AddWorkspaceGroupAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddGroupAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddWorkspaceGroupAccess request, CancellationToken cancellationToken)
    {
        var workspaceGroupAccessRule = await _wikiContext.WorkspaceGroupAccessRules
            .SingleOrDefaultAsync(
                gar => gar.GroupId == request.GroupId &&
                       gar.WorkspaceId == request.WorkspaceId,
                cancellationToken);

        workspaceGroupAccessRule ??= new WorkspaceGroupAccessRule
        {
            GroupId = request.GroupId,
            WorkspaceId = request.WorkspaceId,
        };

        workspaceGroupAccessRule.WorkspaceAccessRule = request.WorkspaceAccessRule;

        _wikiContext.WorkspaceGroupAccessRules.Update(workspaceGroupAccessRule);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}