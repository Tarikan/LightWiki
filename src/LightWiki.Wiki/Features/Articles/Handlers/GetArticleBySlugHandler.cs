using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Extensions;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class GetArticleBySlugHandler : IRequestHandler<GetArticleBySlug, OneOf<ArticleModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;
    private readonly IMapper _mapper;

    public GetArticleBySlugHandler(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository,
        IMapper mapper)
    {
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
        _mapper = mapper;
    }

    public async Task<OneOf<ArticleModel, Fail>> Handle(GetArticleBySlug request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUserOrDefault();
        var article = await _wikiContext.Articles
            .Include(a => a.Versions
                .OrderByDescending(v => v.CreatedAt).Take(1))
            .ThenInclude(av => av.User)
            .Include(a => a.User)
            .Include(a => a.ArticleAccesses)
            .AsNoTracking()
            .SingleAsync(
            a => a.Slug == request.ArticleNameSlug && a.Workspace.Slug == request.WorkspaceNameSlug,
            cancellationToken);

        var idsToSelect = await _articleHierarchyNodeRepository.GetAncestors(article.Id);
        idsToSelect.Add(article.Id);
        List<Article> articles;
        ArticleAccessRule rule;

        if (userContext is null)
        {
            articles = await _wikiContext.Articles
                .IncludeDefaultAccessRules()
                .Where(a => idsToSelect.Contains(a.Id))
                .ToListAsync(cancellationToken);
            rule = articles.Select(a => a.ArticleAccesses.GetHighestPriorityRule())
                .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
        }
        else
        {
            articles = await _wikiContext.Articles
                .IncludeAccessRules(userContext.Id)
                .Where(a => idsToSelect.Contains(a.Id))
                .ToListAsync(cancellationToken);
            rule = articles.Select(a => a.ArticleAccesses.GetHighestPriorityRule())
                .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
        }

        if (!rule.HasFlag(ArticleAccessRule.Read))
        {
            return new Fail("AccessDenied", FailCode.Forbidden);
        }

        var model = _mapper.Map<ArticleModel>(article);
        model.ArticleAccessRuleForCaller = rule;

        return model;
    }
}