using Dasync.Collections;
using ELSA.CodeChallenge.Utilities;
using ELSA.Config;
using ELSA.Repositories.Models;
using Humanizer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ELSA.CodeChallenge.NotificationAPI.Services
{
    public class ELSABackgroundService : IHostedService, IDisposable
    {
        private readonly Lazy<IMongoCollection<ScoreModel>> _scoreCollection;
        //                                               Buffer 1 for arrange, it will keeps collection size and dont make it move to new memory
        public static List<ScoreModel> ScoreModels = new(TAKE_TOP_LEADERBOARD + 1);
        private readonly ISignalRService _signalRService;
        private static readonly int TAKE_TOP_LEADERBOARD = 10;

        public ELSABackgroundService(IApplicationConfig config, ISignalRService signalRService)
        {
            _scoreCollection = new Lazy<IMongoCollection<ScoreModel>>(() =>
            {
                var mongoClient = new MongoClient(config.MongoDbConfig.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.MongoDbConfig.DatabaseNamespace);
                return mongoDatabase.GetCollection<ScoreModel>(GetCollectionName(typeof(ScoreModel)));
            });
            _signalRService = signalRService;
        }
        public void Dispose()
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ScoreModels = await FetchTopScoredAsync();
            WatchAndNotifyLeaderBoardAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ScoreModels.Clear();
            return Task.CompletedTask;
        }

        private async Task WatchAndNotifyLeaderBoardAsync(CancellationToken cancellationToken)
        {
            using (var changeStream = await _scoreCollection.Value.WatchAsync(cancellationToken: cancellationToken, options: new ChangeStreamOptions
            {
                BatchSize = 2,
                MaxAwaitTime = TimeSpan.FromSeconds(3)
            }))
            {
                cancellationToken.ThrowIfCancellationRequested();
                while (await changeStream.MoveNextAsync())
                {
                    bool needPushNotification = false;
                    foreach (var change in changeStream.Current)
                    {
                        switch (change.OperationType)
                        {
                            case ChangeStreamOperationType.Insert:
                                ScoreModels.Add(change.FullDocument);
                                Console.WriteLine("ChangeStreamOperationType.Insert");
                                ArrangeAndLimitScoreList();
                                needPushNotification = true;
                                break;
                            case ChangeStreamOperationType.Update:
                                Console.WriteLine("ChangeStreamOperationType.Update");
                                if (await UpdateAndArrangeListScoreAsync(change))
                                {
                                    Console.WriteLine("Inside handler ChangeStreamOperationType.Update");
                                    needPushNotification = true;
                                }
                                break;
                            case ChangeStreamOperationType.Delete:
                                ScoreModels = await FetchTopScoredAsync();
                                needPushNotification = true;
                                break;
                            default:
                                break;
                        }
                    }

                    if (needPushNotification)
                    {
                        Console.WriteLine("Before " + nameof(_signalRService.BroadcastLeaderboardAsync) + string.Join(Environment.NewLine, ScoreModels.Select(d => d.Username + ": " + d.TotalPoints)));

                        await _signalRService.BroadcastLeaderboardAsync(ScoreModels);
                    }
                }
            }
        }

        private static string GetCollectionName(Type type)
        {
            return type.Name.Pluralize();
        }

        private async Task<List<ScoreModel>> FetchTopScoredAsync()
        {
            var query = await _scoreCollection.Value.FindAsync(Builders<ScoreModel>.Filter.Empty, new FindOptions<ScoreModel, ScoreModel>
            {
                Limit = TAKE_TOP_LEADERBOARD,
                Sort = Builders<ScoreModel>.Sort.Descending(d => d.TotalPoints)
            });
            return [.. await query.ToListAsync()];
        }

        private static void ArrangeAndLimitScoreList()
        {
            ScoreModels = ScoreModels.OrderByDescending(d => d.TotalPoints).DistinctBy(d => d.Id).Take(TAKE_TOP_LEADERBOARD).ToList();
        }

        private async Task<bool> UpdateAndArrangeListScoreAsync(ChangeStreamDocument<ScoreModel> change)
        {
            ObjectId documentId;
            double totalPoints;
            if (
                change.DocumentKey.TryGetValue("_id", out var bsonId) &&
                change.UpdateDescription.UpdatedFields.TryGetValue(NameCollector.Get<ScoreModel>(d => d.TotalPoints), out var bsonTotalPoints)
            )
            {
                documentId = bsonId.AsObjectId;
                totalPoints = bsonTotalPoints.AsDouble;
                if (ScoreModels.Count >= TAKE_TOP_LEADERBOARD)
                {
                    if (ScoreModels.Last().TotalPoints < totalPoints) // Need to update list
                    {
                        var index = ScoreModels.FindIndex(d => d.Id == documentId);
                        if (index > -1)
                        {
                            ScoreModels[index].TotalPoints = totalPoints;
                            return true;
                        }
                        else
                        {
                            if (change.BackingDocument.TryGetValue(NameCollector.Get<ScoreModel>(d => d.UserId), out var bsonUserId))
                            {
                                var userInfos = await GetScoreModelAsync(bsonUserId.AsObjectId);
                                if (userInfos != null)
                                {
                                    ScoreModels.Add(new ScoreModel
                                    {
                                        Id = documentId,
                                        TotalPoints = totalPoints,
                                        UserId = userInfos.Id,
                                        Username = userInfos.Username
                                    });
                                    ArrangeAndLimitScoreList();
                                }

                                return true;
                            }
                            // else => this case no need to update, cause of list already has to TAKE_TOP_LEADERBOARD
                        }
                    }
                }
                else 
                {
                    var userInfos = await GetScoreModelAsync(documentId);
                    if (userInfos != null)
                    {
                        ScoreModels.Add(new ScoreModel
                        {
                            Id = documentId,
                            TotalPoints = totalPoints,
                            UserId = userInfos.Id,
                            Username = userInfos.Username
                        });
                        ArrangeAndLimitScoreList();
                    }

                    return true;
                }
            }

            return false;
        }

        private async Task<ScoreModel> GetScoreModelAsync(ObjectId id)
        {
            return await(await _scoreCollection.Value.FindAsync(x => x.Id == id)).FirstOrDefaultAsync();
        }
    }
}
