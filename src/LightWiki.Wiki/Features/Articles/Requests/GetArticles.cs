using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;
using Sieve.Models;

namespace LightWiki.Features.Articles.Requests
{
    public class GetArticles : SieveModel, IRequest<OneOf<CollectionResult<ArticleModel>, Fail>>
    {
    }
}