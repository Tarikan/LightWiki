namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleHeaderModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentArticleId { get; set; }

    public bool HasChildren { get; set; }

    public string Slug { get; set; }
}