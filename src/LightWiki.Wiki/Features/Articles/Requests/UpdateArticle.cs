using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public sealed class UpdateArticle : IRequest<OneOf<Success, Fail>>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public ArticleAccessRule GlobalAccessRule { get; set; }
}