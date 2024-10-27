using MongoDB.Bson;
using ELSA.DAL.Models.Base;

namespace ELSA.Repositories.Models
{
    public class QuizModel : BaseAuditEntity
    {
        public string Subject { get; set; }
        public ObjectId[] QuestionIds { get; set; } = [];
        public int QuestionLength { get; set; }
    }
}
