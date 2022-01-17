using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using OneOf;

namespace LightWiki.Features.ArticleVersions.Requests;

public class GetArticleVersionContent : IRequest<OneOf<ArticleContentModel, Fail>>
{
    [JsonIgnore]
    public int ArticleVersionId { get; set; }
}