using ELSA.Repositories.Interface;
using ELSA.Repositories.Models;
using ELSA.Services.Interface;
using IdentityServer4.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ELSA.Services
{
    public class ScoreService(IScoreRepository scoreRepository, ILoggedInUserService loggedInUserService) : BaseSimpleService<ScoreModel>(scoreRepository), IScoreService
    {
        public async Task ScoreAsync(ObjectId questionId, double points)
        {
            if (points <= 0)
            {
                return;
            }

            IClientSessionHandle session = await scoreRepository.Client.StartSessionAsync();
            session.StartTransaction();
            try
            {
                var filter = Builders<ScoreModel>.Filter.Eq(d => d.UserId, loggedInUserService.UserId);
                var update = Builders<ScoreModel>.Update.AddToSet(d => d.ScoreByQuesitons, new ScoreByQuestion { QuestionId = questionId, Points = points })
                                                        .SetOnInsert(d => d.Username, loggedInUserService.Username);
                var result = await scoreRepository.UpdateAsync(session, filter, update, new UpdateOptions { IsUpsert = true });
                if (result.ModifiedCount > 0 || result.UpsertedId != null)
                {
                    var updatePoints = Builders<ScoreModel>.Update.Inc(d => d.TotalPoints, points);
                    await scoreRepository.UpdateAsync(session, filter, updatePoints, new UpdateOptions { IsUpsert = true });
                }

                await session.CommitTransactionAsync();
            }
            catch
            {
                await session.AbortTransactionAsync();
            }
        }
    }
}