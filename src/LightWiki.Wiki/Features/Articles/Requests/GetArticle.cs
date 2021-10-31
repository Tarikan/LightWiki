using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests
{
    public sealed class GetArticle : IRequest<OneOf<ArticleWithContentModel, Fail>>
    {
        public int Id { get; set; }
    }
}