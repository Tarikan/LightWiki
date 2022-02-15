using System.Collections.Generic;
using System.Linq;
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
using Sieve.Services;

namespace LightWiki.Features.Workspaces.Handlers;

public class GetWorkspacesHandler : IRequestHandler<GetWorkspaces, OneOf<CollectionResult<WorkspaceModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public GetWorkspacesHandler(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        ISieveProcessor sieveProcessor,
        IMapper mapper)
    {
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
        _sieveProcessor = sieveProcessor;
        _mapper = mapper;
    }

    public async Task<OneOf<CollectionResult<WorkspaceModel>, Fail>> Handle(
        GetWorkspaces request,
        CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUserOrDefault();
        IQueryable<Workspace> workspaceRequest = _wikiContext.Workspaces
            .Include(w => w.RootArticle);

        if (userContext is null)
        {
            workspaceRequest = workspaceRequest
                .IncludeDefaultAccessRules()
                .Where(w => w.WorkspaceAccesses.Single(a => a.Party.PartyType == PartyType.Group &&
                                                            a.Party.Groups.Single().GroupType == GroupType.Default)
                    .WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse));
        }
        else
        {
            workspaceRequest = workspaceRequest
                .IncludeAccessRules(userContext.Id)
                .WhereUserHasAccess(userContext.Id, WorkspaceAccessRule.Browse);
        }

        var count = await workspaceRequest.CountAsync(cancellationToken);
        var results = await _sieveProcessor.Apply(request, workspaceRequest).ToListAsync(cancellationToken);

        var accessRules = results
            .Select(r => new { id = r.Id, rule = r.WorkspaceAccesses.GetHighestLevelRule() })
            .ToList();

        var models = _mapper.Map<List<WorkspaceModel>>(results);

        foreach (var model in models)
        {
            model.WorkspaceAccessRuleForCaller = accessRules.First(r => r.id == model.Id).rule;
        }

        return new CollectionResult<WorkspaceModel>(models, count);
    }
}