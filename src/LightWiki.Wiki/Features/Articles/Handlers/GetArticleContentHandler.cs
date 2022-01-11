using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
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
        var userContext = _authorizedUserProvider.GetUserOrDefault();

        var article = await _wikiContext.Articles
            .Include(a => a.GroupAccessRules
                .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
            .Include(a => a.PersonalAccessRules
                .Where(par => par.UserId == userContext.Id))
            .SingleAsync(a => a.Id == request.ArticleId, cancellationToken);

        var accessLevel = article.GlobalAccessRule;
        if (article.GroupAccessRules.Any())
        {
            accessLevel = article.GroupAccessRules.Single(gar =>
                    (int)gar.ArticleAccessRule == article.GroupAccessRules
                        .Max(gr => (int)gr.ArticleAccessRule))
                .ArticleAccessRule;
        }

        if (article.PersonalAccessRules.Any())
        {
            accessLevel = article.PersonalAccessRules.First().ArticleAccessRule;
        }

        if (accessLevel < ArticleAccessRule.Read)
        {
            return new Fail("User does not have access to this resource", FailCode.Forbidden);
        }

        var text = (await _articleHtmlRepository.GetLatest(request.ArticleId)).Text;

        return new ArticleContentModel
        {
            Text = text,
        };
    }
}