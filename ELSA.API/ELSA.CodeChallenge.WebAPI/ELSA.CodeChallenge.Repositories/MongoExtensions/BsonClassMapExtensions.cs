using MongoDB.Bson.Serialization;
using ELSA.Repositories.Models;

namespace ELSA.Repositories.MongoExtensions
{
    public static class BsonClassMapExtensions
    {
        public static void RegisterBsonClassMap()
        {
            BsonClassMap.RegisterClassMap<QuestionModel>(d =>
            {
                d.AutoMap();
                d.SetIsRootClass(true);
            });
            BsonClassMap.RegisterClassMap<MultiChoiceQuestionModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<SingleChoiceQuestionModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<ImageMultiChoiceQuestionModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<ImageSingleChoiceQuestionModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<EssayQuestionModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<BinaryQuestionModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<QuizModel>(d => d.AutoMap());
            BsonClassMap.RegisterClassMap<UserModel>(d => d.AutoMap());
        }
    }
}
