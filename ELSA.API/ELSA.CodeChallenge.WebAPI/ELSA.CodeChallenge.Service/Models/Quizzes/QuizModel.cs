using MongoDB.Bson;
using ELSA.Services.Models.Questions;

namespace ELSA.Services.Models.Quizzes
{
    public class QuizModel
    {
        public ObjectId Id { get; set; }
        public string Subject { get; set; }
        public List<QuestionModel> Questions { get; set; } = [];
    }
}
