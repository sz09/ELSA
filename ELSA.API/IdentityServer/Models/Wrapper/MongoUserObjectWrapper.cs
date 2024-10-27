using MongoDB.Bson.Serialization.Attributes;
using Humanizer;
using MongoDB.Driver;
using MongoDB.Bson;

namespace IdentityServer.Models.Wrapper;

[BsonIgnoreExtraElements]
public class MongoUserObjectWrapper<T>
{
    public MongoUserObjectWrapper(string userId, T @object)
    {
        UserId = userId;
        Object = @object;
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = new BsonObjectId(ObjectId.GenerateNewId()).ToString();

    public  string UserId { get; set; }
    public T Object { get; set; }

    public static string GetCollectionName()
    {
        var type = typeof(T);
        var typeName = !type.IsGenericType ? type.Name : type.GetGenericArguments()[0].Name;
        if (!typeName.EndsWith("`1"))
        {
            return typeName.Pluralize();
        }
        // Replace("`1", string.Empty) for list, dictionary
        return typeName.Replace("`1", string.Empty).Pluralize();
    }

    public static ProjectionDefinition<MongoUserObjectWrapper<T>, T> ToProjectionDefinition()
    {
        var builders =  Builders<MongoUserObjectWrapper<T>>.Projection;
        return builders.Expression(d => d.Object);
    }
}

public static class MongoUserObjectWrapper
{
    public static MongoUserObjectWrapper<T> Wrap<T>(string userId, T @object)
    {
        return new MongoUserObjectWrapper<T>(userId, @object);
    }

}