using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ELSA.DAL.Models.Base
{
    [BsonIgnoreExtraElements]
    public abstract class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
    }
}
