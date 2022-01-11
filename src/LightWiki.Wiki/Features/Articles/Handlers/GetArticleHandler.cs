using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
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

public sealed class GetArticleHandler : IRequestHandler<GetArticle, OneOf<ArticleModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly AppConfiguration _appConfiguration;

    public GetArticleHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        AppConfiguration appConfiguration)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _appConfiguration = appConfiguration;
    }

    public async Task<OneOf<ArticleModel, Fail>> Handle(
        GetArticle request,
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

        var model = _mapper.Map<ArticleModel>(article);

        return model;
    }
}