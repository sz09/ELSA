using MongoDB.Bson;
using ELSA.Repositories.Models;

namespace ELSA.Services.Interface
{
    public interface IQuestionService : IBaseService<QuestionModel>
    {
        Task<Models.Questions.QuestionModel[]> GetQuestionByQuizAsync(ObjectId quizId);
        Task UpdateQuestionAsync(Models.Questions.QuestionModel questionModel);
        Task<Repositories.Models.QuestionModel> CreateQuestionAsync(Models.Questions.QuestionModel questionModel);
    }
}

