namespace LightWiki.Features.Articles.Requests;

public class RemoveGroupAccess
{
    public int ArticleId { get; set; }

    public int GroupId { get; set; }
}