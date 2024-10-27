using ELSA.Repositories.Models;
using Microsoft.AspNetCore.SignalR;

namespace ELSA.CodeChallenge.NotificationAPI.Services
{
    public interface ISignalRService
    {
        Task BroadcastLeaderboardAsync(List<ScoreModel> scoreModels);
        Task SendBackCallerLeaderboardAsync(ISingleClientProxy singleClientProxy);
    }
}
