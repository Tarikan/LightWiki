using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class GetArticleAncestors : IRequest<OneOf<ArticleAncestorsModel, Fail>>
{
    public int ArticleId { get; set; }
}