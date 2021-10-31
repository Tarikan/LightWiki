using System.Threading.Tasks;
using LightWiki.Data.Mongo.Enums;
using LightWiki.Data.Mongo.Models;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories
{
    public class ArticleMdRepository : BaseRepository<ArticleMd>
    {
        public ArticleMdRepository(MongoSettings mongoSettings) : base(mongoSettings)
        {
        }

        public async Task<ArticleMd> GetLatest(int articleId)
        {
            return await Collection.Find<ArticleMd>(article => article.ArticleId == articleId &&
                                                               article.ArticleStoreType ==
                                                               ArticleStoreType.Latest)
                .SingleOrDefaultAsync();
        }
    }
}