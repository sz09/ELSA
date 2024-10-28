using ELSA.DAL.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ELSA.Repositories.Models
{
    public class ScoreModel : BaseEntity
    {
        public ObjectId UserId { get; set; }
        public string Username { get; set; }
        public double TotalPoints { get; set; }
        public ScoreByQuestion[] ScoreByQuesitons { get; set; } = [];
    }

    [BsonIgnoreExtraElements]
    public class ScoreByQuestion
    {
        public ObjectId QuestionId { get; set; }
        public double Points { get; set; }
    }
}
