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
using LightWiki.Infrastructure.Extensions;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public sealed class CreateArticleHandler : IRequestHandler<CreateArticle, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;

    public CreateArticleHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateArticle request,
        CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces.FindAsync(request.WorkspaceId);

        var userContext = await _authorizedUserProvider.GetUser();
        var article = _mapper.Map<Article>(request);

        article.Name = article.Name.ToUrlFriendlyString();
        article.UserId = userContext.Id;
        article.Versions = new List<ArticleVersion>();

        var version = new ArticleVersion()
        {
            Patch = string.Empty,
            Article = article,
            UserId = userContext.Id,
        };

        article.Versions.Add(version);

        await _wikiContext.AddAsync(version, cancellationToken);
        await _wikiContext.Articles.AddAsync(article, cancellationToken);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var personalAccess = new ArticlePersonalAccessRule
        {
            UserId = userContext.Id,
            ArticleId = article.Id,
            ArticleAccessRule = ArticleAccessRule.All,
        };

        await _wikiContext.ArticlePersonalAccessRules.AddAsync(personalAccess, cancellationToken);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new SuccessWithId<int>(article.Id);
    }
}