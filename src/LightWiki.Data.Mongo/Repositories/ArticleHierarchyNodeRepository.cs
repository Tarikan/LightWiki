using System.Collections.Generic;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;
using LightWiki.Infrastructure.Configuration;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories;

public class ArticleHierarchyNodeRepository : BaseRepository<ArticleHierarchyNode>, IArticleHierarchyNodeRepository
{
    public ArticleHierarchyNodeRepository(IMongoClient mongoClient, ConnectionStrings connectionStrings)
        : base(mongoClient.GetDatabase(connectionStrings.MongoDatabaseName))
    {
    }

    public async Task<List<int>> GetAncestors(int articleId)
    {
        return await Collection
            .Find(a => a.ArticleId == articleId)
            .Project(new ProjectionDefinitionBuilder<ArticleHierarchyNode>()
                .Expression(a => a.AncestorIds))
            .SingleAsync();
    }

    public async Task<List<ArticleHierarchyNode>> GetAncestorModels(int articleId)
    {
        return await Collection.Find(a => a.AncestorIds.Contains(articleId))
            .ToListAsync();
    }

    public async Task<List<ArticleHierarchyNode>> GetAllChildren(int articleId)
    {
        return await Collection
            .Find(a => a.AncestorIds.Contains(articleId))
            .ToListAsync();
    }

    public Task<ArticleHierarchyNode> FindByArticleId(int articleId)
    {
        return Collection
            .Find(a => a.ArticleId == articleId)
            .SingleAsync();
    }
}