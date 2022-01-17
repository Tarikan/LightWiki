using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Features.ArticleVersions.Requests;
using LightWiki.Features.ArticleVersions.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Sieve.Services;

namespace LightWiki.Features.ArticleVersions.Handlers;

public class
    GetArticleVersionsHandler : IRequestHandler<GetArticleVersions, OneOf<CollectionResult<ArticleVersionModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public GetArticleVersionsHandler(WikiContext wikiContext, ISieveProcessor sieveProcessor, IMapper mapper)
    {
        _wikiContext = wikiContext;
        _sieveProcessor = sieveProcessor;
        _mapper = mapper;
    }

    public async Task<OneOf<CollectionResult<ArticleVersionModel>, Fail>> Handle(
        GetArticleVersions request,
        CancellationToken cancellationToken)
    {
        var versions = _wikiContext.ArticleVersions
            .Where(v => v.ArticleId == request.ArticleId)
            .OrderByDescending(v => v.CreatedAt)
            .AsNoTracking();

        var total = await versions.CountAsync(cancellationToken);

        var queryResult = await _sieveProcessor.Apply(request, versions)
            .ToListAsync(cancellationToken);

        return _mapper.Map<CollectionResult<ArticleVersionModel>>(
            new CollectionResult<ArticleVersion>(queryResult, total));
    }
}