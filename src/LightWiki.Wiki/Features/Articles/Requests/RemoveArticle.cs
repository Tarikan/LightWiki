using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class RemoveArticle : IRequest<OneOf<Success, Fail>>
{
    public int ArticleId { get; set; }
}