using MongoDB.Bson.Serialization.Attributes;
using Humanizer;
using MongoDB.Driver;
using MongoDB.Bson;

namespace IdentityServer.Models.Wrapper;

[BsonIgnoreExtraElements]
public class MongoObjectWrapper<T>
{
    public MongoObjectWrapper(T @object)
    {
        Object = @object;
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = new BsonObjectId(ObjectId.GenerateNewId()).ToString();
    public T Object { get; set; }

    public static string GetCollectionName()
    {
        var typeName = typeof(T).Name;
        if (!typeName.EndsWith("`1"))
        {
            return typeName.Pluralize();
        }
        // Replace("`1", string.Empty) for list, dictionary
        return typeName.Replace("`1", string.Empty).Pluralize();
    }

    public static ProjectionDefinition<MongoObjectWrapper<T>, T> ToProjectionDefinition()
    {
        var builders =  Builders<MongoObjectWrapper<T>>.Projection;
        return builders.Expression(d => d.Object);
    }
}

public static class MongoObjectWrapper
{
    public static MongoObjectWrapper<T> Wrap<T>(T @object)
    {
        return new MongoObjectWrapper<T>(@object);
    }

}

internal static class BsonExtensions
{
    internal static string GenerateId()
    {
        return new BsonObjectId(ObjectId.GenerateNewId()).ToString();
    }
}