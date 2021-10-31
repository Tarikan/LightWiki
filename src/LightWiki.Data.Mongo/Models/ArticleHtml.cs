using LightWiki.Data.Mongo.Enums;

namespace LightWiki.Data.Mongo.Models
{
    public class ArticleHtml : BaseModel
    {
        public int ArticleId { get; set; }

        public string Text { get; set; }

        public ArticleStoreType ArticleStoreType { get; set; }

        // for intermediate records
        public int? Index { get; set; }
    }
}