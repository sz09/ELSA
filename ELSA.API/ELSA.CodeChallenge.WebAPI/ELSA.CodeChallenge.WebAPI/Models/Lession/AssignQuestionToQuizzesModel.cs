using MongoDB.Bson;

namespace ELSA.WebAPI.Models.Quiz
{
    public class AssignQuestionToQuizzesModel
    {
        public ObjectId QuestionId { get; set; }
        public ObjectId[] QuizIds { get; set; }
    }
}
