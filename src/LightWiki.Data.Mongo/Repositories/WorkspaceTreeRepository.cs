using LightWiki.Data.Mongo.Models;
using LightWiki.Infrastructure.Configuration;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories;

public class WorkspaceTreeRepository : BaseRepository<WorkspaceTree>, IWorkspaceTreeRepository
{
    public WorkspaceTreeRepository(IMongoClient mongoClient, ConnectionStrings connectionStrings)
        : base(mongoClient.GetDatabase(connectionStrings.MongoDatabaseName))
    {
    }
}