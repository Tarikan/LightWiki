using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Models;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Slugify;

namespace LightWiki.Features.Workspaces.Handlers;

public class CreateWorkspaceHandler : IRequestHandler<CreateWorkspace, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly ISlugHelper _slugHelper;
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;

    public CreateWorkspaceHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        ISlugHelper slugHelper,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _slugHelper = slugHelper;
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateWorkspace request,
        CancellationToken cancellationToken)
    {
        var specialGroups = await _wikiContext.Groups
            .Include(g => g.Party)
            .Where(g => g.GroupType != GroupType.Regular)
            .ToListAsync(cancellationToken);
        var userContext = await _authorizedUserProvider.GetUser();
        var workspace = _mapper.Map<Workspace>(request);
        workspace.Slug = _slugHelper.GenerateSlug(request.Name);

        _wikiContext.Workspaces.Add(workspace);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        var rules = new List<WorkspaceAccess>
        {
            new WorkspaceAccess
            {
                PartyId = specialGroups.Single(g => g.GroupType == GroupType.Admin).PartyId,
                WorkspaceAccessRule = WorkspaceAccessRule.All,
            },
            new WorkspaceAccess
            {
                PartyId = specialGroups.Single(g => g.GroupType == GroupType.Default).PartyId,
                WorkspaceAccessRule = request.WorkspaceAccessRule,
            },
            new WorkspaceAccess
            {
                PartyId = userContext.PartyId,
                WorkspaceAccessRule = WorkspaceAccessRule.All,
            },
        };

        workspace.WorkspaceAccesses = rules;
        _wikiContext.Workspaces.Update(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var rootArticle = new Article
        {
            Name = workspace.Name,
            Slug = workspace.Slug,
            UserId = userContext.Id,
            WorkspaceId = workspace.Id,
            ArticleAccesses = new List<ArticleAccess>
            {
                new ()
                {
                    PartyId = userContext.PartyId,
                    ArticleAccessRule = ArticleAccessRule.All,
                },
                new ()
                {
                    PartyId = specialGroups.Single(g => g.GroupType == GroupType.Default).PartyId,
                    ArticleAccessRule = ArticleAccessRule.Read,
                },
            },
        };

        workspace.RootArticle = rootArticle;

        await _wikiContext.SaveChangesAsync(cancellationToken);

        var hierarchyNode = new ArticleHierarchyNode
        {
            ArticleId = rootArticle.Id,
            WorkspaceId = rootArticle.WorkspaceId,
            ParentId = null,
            AncestorIds = new List<int>(),
        };

        await _articleHierarchyNodeRepository.Create(hierarchyNode);

        return new SuccessWithId<int>(workspace.Id);
    }
}