using ObjectId = MongoDB.Bson.ObjectId;
using Newtonsoft.Json;

namespace ELSA.WebAPI.Helpers
{
    public class ObjectIdConverter : JsonConverter<ObjectId>
    {
        public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return ObjectId.Parse(reader.Value.ToString());
        }
    }
}
