using MongoDB.Bson;
using ELSA.Repositories.Models;

namespace ELSA.Services.Interface
{
    public interface IQuizService : IBaseService<QuizModel>
    {
        Task<Models.Quizzes.QuizModel> GetQuizByIdAsync(ObjectId quizId);
        Task<ObjectId[]> GetQuizScoredQuestionsAsync(ObjectId quizId);

        Task UpdateQuizAsync(Models.Quizzes.UpdateQuizModel updateQuizModel);
        Task DeleteQuizByIdAsync(ObjectId objectId);

        Task<ObjectId[]> GetRelatedQuestionsAsync(ObjectId questionId);
        Task AssignRelatedQuestionAsync(ObjectId quizId, ObjectId questionId);
    }
}

