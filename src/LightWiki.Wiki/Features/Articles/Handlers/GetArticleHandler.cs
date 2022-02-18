using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Extensions;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Auth;
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
        var userContext = await _authorizedUserProvider.GetUserOrDefault();
        var article = await _wikiContext.Articles
            .Include(a => a.Versions
                .OrderByDescending(v => v.CreatedAt).Take(1))
            .ThenInclude(av => av.User)
            .Include(a => a.User)
            .AsNoTracking()
            .SingleAsync(a => a.Id == request.ArticleId, cancellationToken);

        var model = _mapper.Map<ArticleModel>(article);
        model.ArticleAccessRuleForCaller = article.ArticleAccesses.GetHighestPriorityRule();

        return model;
    }
}