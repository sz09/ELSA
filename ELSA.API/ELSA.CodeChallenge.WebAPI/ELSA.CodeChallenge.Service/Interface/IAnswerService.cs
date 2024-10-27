using MongoDB.Bson;
using ELSA.Repositories.Models;

namespace ELSA.Services.Interface
{
    public interface IAnswerService
    {
        Task<bool> AnswerAQuestionAsync(ObjectId questionId, object answer);  
    }
}

