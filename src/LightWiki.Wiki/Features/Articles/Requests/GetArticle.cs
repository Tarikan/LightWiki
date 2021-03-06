using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public sealed class GetArticle : IRequest<OneOf<ArticleModel, Fail>>
{
    public int ArticleId { get; set; }
}