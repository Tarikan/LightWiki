using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Extensions;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Extensions;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Sieve.Services;

namespace LightWiki.Features.Articles.Handlers;

public class GetArticlesHandler : IRequestHandler<GetArticles, OneOf<CollectionResult<ArticleModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly ISieveProcessor _sieveProcessor;

    public GetArticlesHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        ISieveProcessor sieveProcessor)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _sieveProcessor = sieveProcessor;
    }

    public async Task<OneOf<CollectionResult<ArticleModel>, Fail>>
        Handle(GetArticles request, CancellationToken cancellationToken)
    {
        var userContext = _authorizedUserProvider.GetUserOrDefault();
        IQueryable<Article> query;

        if (userContext is null)
        {
            query = _wikiContext.Articles
                .Include(a => a.ArticleAccesses)
                .WhereDefaultUserHasAccess(ArticleAccessRule.Read)
                .AsNoTracking();
        }
        else
        {
            query = _wikiContext.Articles
                    .WhereUserHasAccess(userContext.Id, ArticleAccessRule.Read)
                    .Include(a => a.ArticleAccesses)
                    .AsNoTracking();
        }

        var total = await query.CountAsync(cancellationToken);

        var result = await _sieveProcessor.Apply(request, query).ToCollectionResult(total, cancellationToken);

        return _mapper.Map<CollectionResult<ArticleModel>>(result);
    }
}