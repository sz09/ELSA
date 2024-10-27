using MongoDB.Bson;
namespace ELSA.WebAPI.Models.Answer
{
    public class AnswerModel
    {
        public ObjectId QuestionId { get; set; }
        public object Answer { get; set; }
    }
}
