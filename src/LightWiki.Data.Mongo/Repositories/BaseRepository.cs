using System.Collections.Generic;
using LightWiki.Data.Mongo.Models;
using MongoDB.Driver;

namespace LightWiki.Data.Mongo.Repositories
{
    public abstract class BaseRepository<T>
        where T : BaseModel
    {
        protected IMongoCollection<T> Collection { get; }

        protected BaseRepository(MongoSettings mongoSettings)
        {
            var client = new MongoClient(mongoSettings.ConnectionString);
            var database = client.GetDatabase(mongoSettings.DatabaseName);
            Collection = database.GetCollection<T>(nameof(T));
        }

        public List<T> Get() =>
            Collection.Find(entity => true).ToList();

        public T Get(string id) =>
            Collection.Find<T>(entity => entity.Id == id).FirstOrDefault();

        public T Create(T entity)
        {
            Collection.InsertOne(entity);
            return entity;
        }

        public void Update(string id, T entityIn) =>
            Collection.ReplaceOne(entity => entity.Id == id, entityIn);

        public void Remove(T entityIn) =>
            Collection.DeleteOne(entity => entity.Id == entityIn.Id);

        public void Remove(string id) =>
            Collection.DeleteOne(entity => entity.Id == id);
    }
}