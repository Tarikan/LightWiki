using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
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

    public CreateArticleHandler(WikiContext wikiContext, IMapper mapper, IAuthorizedUserProvider authorizedUserProvider)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateArticle request,
        CancellationToken cancellationToken)
    {
        var userContext = _authorizedUserProvider.GetUser();
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

        return new SuccessWithId<int>(article.Id);
    }
}