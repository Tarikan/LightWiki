namespace LightWiki.Data.Mongo.Models;

public class ArticlePatchModel : BaseModel
{
    public string Patch { get; set; }

    public int ArticleVersionId { get; set; }
}