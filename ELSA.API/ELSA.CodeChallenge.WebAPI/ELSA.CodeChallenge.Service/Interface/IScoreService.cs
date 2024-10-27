using ELSA.Repositories.Models;
using MongoDB.Bson;

namespace ELSA.Services.Interface
{
    public interface IScoreService : IBaseSimpleService<ScoreModel>
    {
        Task ScoreAsync(ObjectId questionId, double points);
    }
}

