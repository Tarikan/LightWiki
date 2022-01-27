using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
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
        IQueryable<Workspace> workspaceRequest;

        if (userContext is null)
        {
            workspaceRequest = _wikiContext.Workspaces
                .Where(w => w.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse));
        }
        else
        {
            workspaceRequest = _wikiContext.Workspaces
                .Include(w => w.RootArticle)
                .Include(w => w.PersonalAccessRules
                    .Where(par => par.UserId == userContext.Id))
                .Include(w => w.GroupAccessRules
                    .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                .Where(w => w.PersonalAccessRules.Any(r => r.UserId == userContext.Id) &&
                            w.PersonalAccessRules.First(r => r.UserId == userContext.Id)
                                .WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse) ||
                            w.PersonalAccessRules.All(r => r.UserId != userContext.Id) &&
                            w.GroupAccessRules.Any(r => r.Group.Users.Any(u => u.Id == userContext.Id) &&
                                                        r.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse)) ||
                            w.PersonalAccessRules.All(r => r.UserId != userContext.Id) &&
                            w.GroupAccessRules
                                .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id))
                                .All(gar => !gar.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse)) &&
                            w.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse));
        }

        var count = await workspaceRequest.CountAsync(cancellationToken);
        var results = await _sieveProcessor.Apply(request, workspaceRequest).ToListAsync(cancellationToken);

        var models = _mapper.Map<CollectionResult<WorkspaceModel>>(new CollectionResult<Workspace>(results, count));

        return models;
    }
}