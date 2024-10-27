using MongoDB.Bson;

namespace ELSA.Services.Models.Quizzes
{
    public class UpdateQuizModel
    {
        public ObjectId Id { get; set; }
        public string Subject { get; set; }
    }
}
