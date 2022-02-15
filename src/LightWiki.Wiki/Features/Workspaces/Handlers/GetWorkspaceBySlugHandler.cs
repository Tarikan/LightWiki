using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Extensions;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class GetWorkspaceBySlugHandler : IRequestHandler<GetWorkspaceBySlug, OneOf<WorkspaceModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IMapper _mapper;

    public GetWorkspaceBySlugHandler(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IMapper mapper)
    {
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
        _mapper = mapper;
    }

    public async Task<OneOf<WorkspaceModel, Fail>> Handle(
        GetWorkspaceBySlug request,
        CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUserOrDefault();
        Workspace workspace;

        if (userContext is null)
        {
            workspace = await _wikiContext.Workspaces
                .Include(w => w.RootArticle)
                .SingleOrDefaultAsync(w => w.Slug == request.Slug, cancellationToken);
            if (workspace is null)
            {
                return new Fail("Workspace not found", FailCode.NotFound);
            }

            if (!workspace.WorkspaceAccesses.GetHighestLevelRule().HasFlag(WorkspaceAccessRule.Browse))
            {
                return new Fail("Access denied", FailCode.Forbidden);
            }
        }
        else
        {
            workspace = await _wikiContext.Workspaces
                .Include(w => w.RootArticle)
                .IncludeAccessRules(userContext.Id)
                .SingleOrDefaultAsync(w => w.Slug == request.Slug, cancellationToken);
        }

        if (workspace is null)
        {
            return new Fail("Workspace not found", FailCode.NotFound);
        }

        var rule = workspace.WorkspaceAccesses.GetHighestLevelRule();
        if (!rule.HasFlag(WorkspaceAccessRule.Browse))
        {
            return new Fail("Access denied", FailCode.Forbidden);
        }

        var result = _mapper.Map<WorkspaceModel>(workspace);
        result.WorkspaceAccessRuleForCaller = rule;

        return result;
    }
}