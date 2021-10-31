using System.Threading.Tasks;
using LightWiki.Data.Mongo.Enums;
using LightWiki.Data.Mongo.Models;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories
{
    public class ArticleHtmlRepository : BaseRepository<ArticleHtml>
    {
        public ArticleHtmlRepository(MongoSettings mongoSettings) : base(mongoSettings)
        {
        }

        public async Task<ArticleHtml> GetLatest(int articleId)
        {
            return await Collection.Find<ArticleHtml>(article => article.ArticleId == articleId &&
                                                                       article.ArticleStoreType ==
                                                                       ArticleStoreType.Latest)
                .SingleOrDefaultAsync();
        }
    }
}