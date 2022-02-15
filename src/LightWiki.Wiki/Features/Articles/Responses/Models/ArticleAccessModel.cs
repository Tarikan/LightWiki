using LightWiki.Domain.Enums;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleAccessModel
{
    public int ArticleId { get; set; }

    public int PartyId { get; set; }

    public ArticleAccessRule ArticleAccessRule { get; set; }
}