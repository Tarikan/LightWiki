using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class RemovePersonalAccess : IRequest<OneOf<Success, Fail>>
{
    public int ArticleId { get; set; }

    public int UserId { get; set; }
}