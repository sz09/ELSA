using ELSA.CodeChallenge.NotificationAPI.Models;
using ELSA.Repositories.Models;
using Microsoft.AspNetCore.SignalR;

namespace ELSA.CodeChallenge.NotificationAPI.Services
{

    public class SignalRService: ISignalRService
    {
        private readonly IHubContext<SingalRHub> _hubContext;
        public SignalRService(IHubContext<SingalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastLeaderboardAsync(List<ScoreModel> scoreModels)
        {
            await _hubContext.Clients.All.SendAsync("LeaderboardChange", Convert(scoreModels));
            Console.WriteLine(nameof(BroadcastLeaderboardAsync) + string.Join(Environment.NewLine, scoreModels.Select(d => d.Username + ": " + d.TotalPoints)));
        }

        public async Task SendBackCallerLeaderboardAsync(ISingleClientProxy singleClientProxy)
        {
            var x = ELSABackgroundService.ScoreModels;
            await singleClientProxy.SendAsync("LeaderboardChange", Convert(x));
            Console.WriteLine(nameof(SendBackCallerLeaderboardAsync) + string.Join(Environment.NewLine, x.Select(d => d.Username + ": " + d.TotalPoints)));
        }


        private List<SignalRScoreModel> Convert(List<ScoreModel> scoreModels)
        {
            return scoreModels.Select(d => new SignalRScoreModel
            {
                uid = d.UserId.ToString(),
                p = d.TotalPoints,
                un = d.Username
            }).ToList();
        }
    }
}
