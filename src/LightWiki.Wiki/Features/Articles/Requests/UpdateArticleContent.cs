using LightWiki.Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class UpdateArticleContent : IRequest<OneOf<Success, Fail>>
{
    [JsonIgnore]
    public int ArticleId { get; set; }

    public string Text { get; set; }
}