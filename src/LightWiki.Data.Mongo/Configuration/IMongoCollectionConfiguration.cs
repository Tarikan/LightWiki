using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;

namespace LightWiki.Data.Mongo.Configuration;

public interface IMongoCollectionConfiguration<TModel>
    where TModel : BaseModel
{
    Task Configure();
}