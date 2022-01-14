using LightWiki.Domain.Enums;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticlePersonalAccessRuleModel
{
    public int Id { get; set; }

    public ArticleAccessRule ArticleAccessRule { get; set; }

    public int UserId { get; set; }
}