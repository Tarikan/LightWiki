using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public sealed class CreateArticle : IRequest<OneOf<SuccessWithId<int>, Fail>>
{
    public string Name { get; set; }

    public ArticleAccessRule GlobalAccessRule { get; set; }
}