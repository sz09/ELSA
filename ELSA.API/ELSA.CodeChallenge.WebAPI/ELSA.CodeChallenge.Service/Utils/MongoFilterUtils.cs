using ELSA.CodeChallenge.Utilities;
using ELSA.Repositories.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Reflection;

namespace ELSA.CodeChallenge.Services.Utils
{
    public static class MongoFilterUtils
    {
        private static string[] ExcludeFileds = [
            NameCollector.Get<QuestionModel>(d => d.Id),
            NameCollector.Get<QuestionModel>(d => d.Type), // Dont change type
            NameCollector.Get<QuestionModel>(d => d.UpdatedAt),
            NameCollector.Get<QuestionModel>(d => d.CreatedAt),
            NameCollector.Get<QuestionModel>(d => d.CreatedBy),
            NameCollector.Get<QuestionModel>(d => d.UpdatedBy),
        ];
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _caches = new ConcurrentDictionary<Type, PropertyInfo[]>();
        public static UpdateDefinition<QuestionModel> ExtractUpdateEntity(this QuestionModel model)
        {
            var properties = _caches.GetOrAdd(model.GetType(), (type) => type.GetProperties().Where(d => d.CanWrite && !ExcludeFileds.Contains(d.Name)).ToArray());
            List<UpdateDefinition<QuestionModel>> updates = new(properties.Length);
            foreach (var property in properties)
            {
                updates.Add(Builders<QuestionModel>.Update.Set(property.Name, property.GetValue(model)));
            }

            return Builders<QuestionModel>.Update.Combine(updates);
        }
    }
}
