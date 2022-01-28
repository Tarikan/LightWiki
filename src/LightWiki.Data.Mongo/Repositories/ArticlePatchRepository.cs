using System.Collections.Generic;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;
using LightWiki.Infrastructure.Configuration;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories;

public class ArticlePatchRepository : BaseRepository<ArticlePatchModel>, IArticlePatchRepository
{
    public ArticlePatchRepository(IMongoClient mongoClient, ConnectionStrings connectionStrings)
        : base(mongoClient.GetDatabase(connectionStrings.MongoDatabaseName))
    {
    }

    public async Task<List<ArticlePatchModel>> GetByVersionIds(IList<int> articleVersionIds)
    {
        return await Collection.Find(v => articleVersionIds.Contains(v.ArticleVersionId))
            .SortBy(v => v.ArticleVersionId)
            .ToListAsync();
    }
}