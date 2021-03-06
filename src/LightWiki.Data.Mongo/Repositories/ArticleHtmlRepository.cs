using System.Threading.Tasks;
using LightWiki.Data.Mongo.Enums;
using LightWiki.Data.Mongo.Models;
using LightWiki.Infrastructure.Configuration;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories;

public class ArticleHtmlRepository : BaseRepository<ArticleHtml>, IArticleHtmlRepository
{
    public ArticleHtmlRepository(IMongoClient mongoClient, ConnectionStrings connectionStrings)
        : base(mongoClient.GetDatabase(connectionStrings.MongoDatabaseName))
    {
    }

    public async Task<ArticleHtml> GetLatest(int articleId)
    {
        return await Collection.Find(article => article.ArticleId == articleId &&
                                                article.ArticleStoreType ==
                                                ArticleStoreType.Latest)
            .SingleOrDefaultAsync();
    }
}