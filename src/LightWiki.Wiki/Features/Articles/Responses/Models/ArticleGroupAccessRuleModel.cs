using LightWiki.Domain.Enums;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleGroupAccessRuleModel
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public ArticleAccessRule ArticleAccessRule { get; set; }
}