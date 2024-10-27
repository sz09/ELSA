using Microsoft.AspNetCore.SignalR;

namespace ELSA.CodeChallenge.NotificationAPI.Services
{
    public class SingalRHub : Hub
    {
        private readonly ISignalRService _signalRService;
        public SingalRHub(ISignalRService signalRService)
        {
            _signalRService = signalRService;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await _signalRService.SendBackCallerLeaderboardAsync(Clients.Caller);
        }
    }
}
