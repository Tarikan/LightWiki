using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Configuration;

public abstract class MongoCollectionConfigurationBase<TModel> : IMongoCollectionConfiguration<TModel>
    where TModel : BaseModel
{
    protected IMongoCollection<TModel> Collection { get; }

    protected MongoCollectionConfigurationBase(IMongoDatabase database)
    {
        Collection = database.GetCollection<TModel>(typeof(TModel).Name);
    }

    public abstract Task Configure();
}