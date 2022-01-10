using System.Collections.Generic;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;
using LightWiki.Infrastructure.Configuration;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : BaseModel
    {
        protected IMongoCollection<T> Collection { get; }

        protected BaseRepository(MongoConfiguration mongoSettings)
        {
            var client = new MongoClient(mongoSettings.ConnectionString);
            var database = client.GetDatabase(mongoSettings.DatabaseName);
            Collection = database.GetCollection<T>(nameof(T));
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

        public async Task Update(string id, T entityIn) =>
            Collection.ReplaceOneAsync(entity => entity.Id == id, entityIn);

        public Task Remove(T entityIn) =>
            Collection.DeleteOneAsync(entity => entity.Id == entityIn.Id);

        public Task Remove(string id) =>
            Collection.DeleteOneAsync(entity => entity.Id == id);
    }
}