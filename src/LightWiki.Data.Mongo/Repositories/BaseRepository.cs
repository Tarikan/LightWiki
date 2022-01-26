using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T>
    where T : BaseModel
{
    protected IMongoCollection<T> Collection { get; }

    protected BaseRepository(IMongoDatabase database)
    {
        Collection = database.GetCollection<T>(typeof(T).Name);
    }

    public Task<List<T>> Get() =>
        Collection.Find(entity => true).ToListAsync();

    public Task<T> Get(string id) =>
        Collection.Find<T>(entity => entity.Id == id).FirstOrDefaultAsync();

    public async Task<T> Create(T entity)
    {
        await Collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<string> Update(string id, T entityIn)
    {
        var replaceResult = await Collection.ReplaceOneAsync(entity => entity.Id == id, entityIn);

        return entityIn.Id;
    }

    public Task Remove(T entityIn) =>
        Collection.DeleteOneAsync(entity => entity.Id == entityIn.Id);

    public Task Remove(string id) =>
        Collection.DeleteOneAsync(entity => entity.Id == id);

    public async Task RemoveMany(IEnumerable<string> ids)
    {
        var filter = new FilterDefinitionBuilder<T>()
            .Where(a => ids.Contains(a.Id));
        await Collection.DeleteManyAsync(filter);
    }
}