using MongoDB.Bson;

namespace ELSA.WebAPI.ViewModels
{
    public class ViewQuizModel
    {
        public ObjectId Id { get; set; }
        public string Subject { get; set; }
        public ObjectId[] QuestionIds { get; set; }
    }
}