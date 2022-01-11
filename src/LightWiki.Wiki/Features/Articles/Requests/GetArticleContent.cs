using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class GetArticleContent : IRequest<OneOf<ArticleContentModel, Fail>>
{
    public int ArticleId { get; set; }
}