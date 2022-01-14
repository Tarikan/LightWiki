using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class AddGroupAccess : IRequest<OneOf<Success, Fail>>
{
    public int ArticleId { get; set; }

    public int GroupId { get; set; }

    public ArticleAccessRule AccessRule { get; set; }
}