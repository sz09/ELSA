using MongoDB.Bson;

namespace ELSA.WebAPI.Models.Quiz
{
    public class UpdateQuizModel
    {
        public ObjectId Id { get; set; }
        public string Subject { get; set; }
    }
}
