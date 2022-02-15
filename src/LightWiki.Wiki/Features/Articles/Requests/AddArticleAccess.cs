using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class AddArticleAccess : IRequest<OneOf<Success, Fail>>
{
    public int PartyId { get; set; }

    public int ArticleId { get; set; }

    public ArticleAccessRule ArticleAccessRule { get; set; }
}