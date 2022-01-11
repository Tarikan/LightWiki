using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LightWiki.Data.Mongo.Models;

public abstract class BaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}