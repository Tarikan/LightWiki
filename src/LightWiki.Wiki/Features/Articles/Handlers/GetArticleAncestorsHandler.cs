using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
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

public class GetArticleAncestorsHandler : IRequestHandler<GetArticleAncestors, OneOf<ArticleAncestorsModel, Fail>>
{
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;

    public GetArticleAncestorsHandler(
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository,
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<ArticleAncestorsModel, Fail>> Handle(GetArticleAncestors request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUserOrDefault();
        var ancestorIds = await _articleHierarchyNodeRepository.GetAncestors(request.ArticleId);
        List<Article> articles;
        ArticleAccessRule rule;

        var idsToSelect = ancestorIds.Append(request.ArticleId);

        if (userContext is null)
        {
            articles = await _wikiContext.Articles
                .Where(a => idsToSelect.Contains(a.Id))
                .ToListAsync(cancellationToken);
            rule = articles.Select(a => a.GlobalAccessRule)
                .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
        }
        else
        {
            articles = await _wikiContext.Articles
                .Include(a => a.GroupAccessRules
                    .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                .Include(a => a.PersonalAccessRules
                    .Where(par => par.UserId == userContext.Id))
                .Where(a => idsToSelect.Contains(a.Id))
                .ToListAsync(cancellationToken);
            rule = articles.Select(a => a.GetHighestPriorityRule())
                .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
        }

        var test = await _articleHierarchyNodeRepository.GetTree(request.ArticleId);

        if (!rule.HasFlag(ArticleAccessRule.Read))
        {
            return new Fail("Access Denied", FailCode.Forbidden);
        }

        return new ArticleAncestorsModel
        {
            ArticleId = request.ArticleId,
            ArticleAncestors = ancestorIds,
        };
    }
}