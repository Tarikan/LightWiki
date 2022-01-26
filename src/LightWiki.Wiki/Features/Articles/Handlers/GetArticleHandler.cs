﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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

public sealed class GetArticleHandler : IRequestHandler<GetArticle, OneOf<ArticleModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;

    public GetArticleHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
    }

    public async Task<OneOf<ArticleModel, Fail>> Handle(
        GetArticle request,
        CancellationToken cancellationToken)
    {
        var userContext = _authorizedUserProvider.GetUserOrDefault();
        var idsToSelect = await _articleHierarchyNodeRepository.GetAncestors(request.ArticleId);
        idsToSelect.Add(request.ArticleId);
        List<Article> articles;
        ArticleAccessRule rule;

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
                .Include(a => a.PersonalAccessRules
                    .Where(par => par.UserId == userContext.Id))
                .Include(a => a.GroupAccessRules
                    .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                .Where(a => idsToSelect.Contains(a.Id))
                .ToListAsync(cancellationToken);

            rule = articles.Select(a => a.GetHighestPriorityRule())
                .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
        }

        if (!rule.HasFlag(ArticleAccessRule.Read))
        {
            return new Fail("AccessDenied", FailCode.Forbidden);
        }

        var model = _mapper.Map<ArticleModel>(articles.Single(a => a.Id == request.ArticleId));

        return model;
    }
}