using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Extensions;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class GetArticleContentHandler : IRequestHandler<GetArticleContent, OneOf<ArticleContentModel, Fail>>
{
    private readonly IArticleHtmlRepository _articleHtmlRepository;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly WikiContext _wikiContext;
    private readonly AppConfiguration _appConfiguration;

    public GetArticleContentHandler(
        IArticleHtmlRepository articleHtmlRepository,
        IAuthorizedUserProvider authorizedUserProvider,
        WikiContext wikiContext,
        AppConfiguration appConfiguration)
    {
        _articleHtmlRepository = articleHtmlRepository;
        _authorizedUserProvider = authorizedUserProvider;
        _wikiContext = wikiContext;
        _appConfiguration = appConfiguration;
    }

    public async Task<OneOf<ArticleContentModel, Fail>> Handle(
        GetArticleContent request,
        CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUserOrDefault();
        Article article;

        if (userContext is null)
        {
            article = await _wikiContext.Articles
                .IncludeDefaultAccessRules()
                .SingleAsync(a => a.Id == request.ArticleId, cancellationToken);
        }
        else
        {
            article = await _wikiContext.Articles
                .IncludeAccessRules(userContext.Id)
                .SingleAsync(a => a.Id == request.ArticleId, cancellationToken);
        }

        var accessLevel = article.ArticleAccesses.GetHighestPriorityRule();

        if (!accessLevel.HasFlag(ArticleAccessRule.Read))
        {
            return new Fail("User does not have access to this resource", FailCode.Forbidden);
        }

        var text = await _articleHtmlRepository.GetLatest(request.ArticleId);

        return new ArticleContentModel
        {
            Text = text == null ? string.Empty : text.Text,
        };
    }
}