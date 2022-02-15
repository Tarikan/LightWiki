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
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Slugify;

namespace LightWiki.Features.Articles.Handlers;

public sealed class CreateArticleHandler : IRequestHandler<CreateArticle, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;
    private readonly ISlugHelper _slugHelper;

    public CreateArticleHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository,
        ISlugHelper slugHelper)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
        _slugHelper = slugHelper;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateArticle request,
        CancellationToken cancellationToken)
    {
        var specialGroups = await _wikiContext.Groups
            .Where(g => g.GroupType != GroupType.Regular)
            .ToListAsync(cancellationToken);

        var userContext = await _authorizedUserProvider.GetUser();
        var article = _mapper.Map<Article>(request);
        article.Slug = _slugHelper.GenerateSlug(request.Name);

        article.Name = article.Name;
        article.UserId = userContext.Id;
        article.Versions = new List<ArticleVersion>();

        var version = new ArticleVersion()
        {
            Article = article,
            UserId = userContext.Id,
        };

        article.Versions.Add(version);

        await _wikiContext.AddAsync(version, cancellationToken);
        await _wikiContext.Articles.AddAsync(article, cancellationToken);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var rules = new List<ArticleAccess>
        {
            new ()
            {
                PartyId = userContext.PartyId,
                ArticleAccessRule = ArticleAccessRule.All,
            },
            new ()
            {
                PartyId = specialGroups.Single(g => g.GroupType == GroupType.Admin).PartyId,
                ArticleAccessRule = ArticleAccessRule.All,
            },
            new ()
            {
                PartyId = specialGroups.Single(g => g.GroupType == GroupType.Default).PartyId,
                ArticleAccessRule = request.GlobalAccessRule,
            },
        };

        article.ArticleAccesses = rules;
        await _wikiContext.SaveChangesAsync(cancellationToken);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        var parentAncestors = new List<int>();
        if (request.ParentId.HasValue)
        {
            parentAncestors = await _articleHierarchyNodeRepository.GetAncestors(request.ParentId.Value);
            parentAncestors.Add(request.ParentId.Value);
        }

        var hierarchyNode = new ArticleHierarchyNode
        {
            ArticleId = article.Id,
            WorkspaceId = article.WorkspaceId,
            ParentId = request.ParentId,
            AncestorIds = parentAncestors,
        };

        await _articleHierarchyNodeRepository.Create(hierarchyNode);

        return new SuccessWithId<int>(article.Id);
    }
}