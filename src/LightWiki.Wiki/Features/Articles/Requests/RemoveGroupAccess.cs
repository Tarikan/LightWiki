using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class RemoveGroupAccess : IRequest<OneOf<Success, Fail>>
{
    public int ArticleId { get; set; }

    public int GroupId { get; set; }
}