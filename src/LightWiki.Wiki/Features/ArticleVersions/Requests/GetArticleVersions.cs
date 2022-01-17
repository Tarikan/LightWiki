using LightWiki.Features.ArticleVersions.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;
using Sieve.Models;

namespace LightWiki.Features.ArticleVersions.Requests;

public class GetArticleVersions : SieveModel, IRequest<OneOf<CollectionResult<ArticleVersionModel>, Fail>>
{
    public int ArticleId { get; set; }
}