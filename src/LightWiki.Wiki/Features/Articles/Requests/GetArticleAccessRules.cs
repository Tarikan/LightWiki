using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using OneOf;
using Sieve.Models;

namespace LightWiki.Features.Articles.Requests;

public class GetArticleAccessRules : SieveModel, IRequest<OneOf<CollectionResult<ArticleAccessRuleModel>, Fail>>
{
    [JsonIgnore]
    public int ArticleId { get; set; }
}