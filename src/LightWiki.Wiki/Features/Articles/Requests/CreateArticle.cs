using System.Collections.Generic;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public sealed class CreateArticle : IRequest<OneOf<SuccessWithId<int>, Fail>>
{
    public int WorkspaceId { get; set; }

    public List<int> ParentIds { get; set; }

    public string Name { get; set; }

    public ArticleAccessRule GlobalAccessRule { get; set; }
}